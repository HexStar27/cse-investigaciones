using CSE;
using UnityEngine;
using UnityEngine.Events;

namespace Hexstar.CSE
{
	/// <summary>
	/// Esta clase se va a encargar de mover el bloque y de realizar y comprobar conexiones
	/// </summary>
	public class BlockMovAndConexion : MonoBehaviour
	{
		protected Camera cam;
		protected Transform trans;
		protected Collider coll;
		protected AudioSource audioS;
		public ContentQueryUnit CQU { get; private set; }

		[SerializeField] BlockData bd;
		[SerializeField] BlockMovConfig config;
		[SerializeField] Transform bloqueCuerpo;

		bool wantToDeleteBlock;
		GameObject currentGhost;
		GameObject currentDeletionBlock;
		Conector oldConector = null;
		protected static readonly float block2UnitScale = 0.1f;
		public float connectorOffset = 0.05f;
		[SerializeField] float ghostBlockScale = 0.5f;

		[SerializeField] bool cancelGrab = false;
		[SerializeField] bool sendXAPIStatement = true;
		[HideInInspector] public UnityEvent onBlockConectionChanged = new UnityEvent();

		[Header("Conectores")]
		[SerializeField] Conector cT;
		[SerializeField] Conector cL,cB,cR;

		#region MovementVars
		protected Vector3 mOffset;
		protected float mZCoord;
		protected Vector3 initMousePos;
		protected Vector3 iniPos;
		private float initMouseBlockDistance = 0;
		protected bool click = true;
		protected bool beginDrag = false;
		private float oldHeight;

		protected static float maxDist = 40;
		[Header("Movement related")]
		[SerializeField] protected bool useLimitBox;
		private Bounds limitBox;
		#endregion

		//Para type checking
		private BlockType seccionPresente = null;

		public string GetBlockTitle() => bd.title;

		#region Drag
		private void BeginDrag()
		{
			mZCoord = cam.WorldToScreenPoint(trans.position).z;
			var mp = GetMouseWorldPos();
			iniPos = trans.position;
			mOffset = trans.position - mp;
			initMouseBlockDistance = trans.position.x - mp.x;

			//Pasa info al resto de que puede que haya cambiado de sección
			if (DisconectEntryConectors()) {
				audioS.Stop();
				audioS.PlayOneShot(config.blockDisconect);
				RefreshSeccionForEntireGroup(seccionPresente);
			}
			//CollisionEnabling(false);
		}
		private void KeepDrag()
		{
			trans.position = Constrain(GetPointOfCollision()) + new Vector3(0, config.grabHeight, 0);

			//Logic for the positioning fo the ghost block
			Conector c = CalculateClosestConection();
			if(oldConector == c) 
			{
				if (c == null && currentGhost != null) Destroy(currentGhost);
				else if(currentGhost != null)
				{
					Conector salida = c.TocandoCercano();
					Conector ghostBlockParent = c;
					if (FormaParteDelChunk(c)) ghostBlockParent = salida;

					currentGhost.transform.parent = ghostBlockParent.transform.parent;
					
					currentGhost.transform.localPosition = 
						salida.ElBloque().CalculatePositionForConectedBlock(ghostBlockParent,c);
					currentGhost.transform.localScale = new Vector3(bloqueCuerpo.localScale.x * ghostBlockScale, 1, 1);
				}
			}
			else if (c != null)
			{
				if (currentGhost != null) Destroy(currentGhost);
				Conector salida = c.TocandoCercano();
				Conector ghostBlockParent = c;
				if (FormaParteDelChunk(c)) ghostBlockParent = salida;

				currentGhost = Instantiate(config.ghostBlock, ghostBlockParent.transform.parent);
				currentGhost.transform.localPosition = 
					salida.ElBloque().CalculatePositionForConectedBlock(ghostBlockParent,c);
				currentGhost.transform.localScale = new Vector3(bloqueCuerpo.localScale.x * ghostBlockScale, 1, 1);
			}
			oldConector = c;

			CheckDistanceForDeletion();
			//Logic for the deletion of the block
			if (currentGhost == null && wantToDeleteBlock)
            {
				if(currentDeletionBlock == null) currentDeletionBlock = Instantiate(config.deletionBlock, trans, false);
				currentDeletionBlock.transform.localScale = new Vector3(bloqueCuerpo.localScale.x * ghostBlockScale, 1, 1);
            }
            else if (currentDeletionBlock != null) Destroy(currentDeletionBlock);
		}
		#endregion

		private bool MouseMoved()
		{
			click = click && initMousePos == Input.mousePosition;
			return click;
		}
		public bool IsClick() { return click; }
		public Vector3 CalculatePositionForConectedBlock(Conector cSalida, Conector cEntrada)
		{
			var s = cEntrada.ElBloque().GetBlockSize();
			//Suponemos que la funcion se ha llamado con el bloque perteneciente a la salida
			float x = block2UnitScale * (bloqueCuerpo.localScale.x + s) + connectorOffset;
			float z = 0.15f;

			switch (cSalida.Tipo())
			{
				case Conector.ConexionType.TOP:		return new Vector3( 0, 0, z);
				case Conector.ConexionType.LEFT:	return new Vector3(-x, 0, 0);
				case Conector.ConexionType.BOTTOM:	return new Vector3( 0, 0,-z);
				case Conector.ConexionType.RIGHT:	return new Vector3( x, 0, 0);
				default: return Vector3.zero;
			}
		}
		public float GetBlockSize() { return bloqueCuerpo.transform.localScale.x; }

		public bool TieneSalidaConectada()
        {
			return cR.EstaConectado() || cB.EstaConectado();
        }

		public bool DisconectEntryConectors()
		{
			BlockMovAndConexion opuesto = null;
			bool desconexion = cT.EstaConectado() || cL.EstaConectado();
			cT.AplicarConexion(false);
			if(cL.EstaConectado()) opuesto = cL.ConectorConectado().ElBloque();
			cL.AplicarConexion(false);

			BuscarTipoSeccion();
			HundirOnConnectionChange();
			if (opuesto != null) opuesto.FlotarOnConnectionChange();
			
			trans.parent = null;
			return desconexion;
		}
		public void CollisionEnabling(bool value)
		{
			coll.enabled = value;
		}

		private void CheckDistanceForDeletion()
        {
			float mouseBlockDistance = trans.position.x - GetMouseWorldPos().x;
			wantToDeleteBlock = Mathf.Abs(initMouseBlockDistance - mouseBlockDistance) > config.deletionThreshold;
		}

		public void RefreshSeccionForEntireGroup(BlockType seccionActual)
        {
			seccionPresente = seccionActual;
			if(cR.EstaConectado())
            {
				cR.ConectorConectado().ElBloque().RefreshSeccionForEntireGroup(seccionActual);
            }
        }

		public BlockData GetBlockData() { return bd; }
		public void SetBlockData(BlockData bd) { this.bd = bd; }

		public static bool CheckConexionType(Conector salida, Conector entrada)
		{
			var infoEntrada = entrada.ElBloque().GetBlockData();
			var infoSalida = salida.ElBloque().GetBlockData();
			if (infoEntrada == null || infoSalida == null) return true;
			
			var filtro = infoSalida.salidaDerecha;
			if (entrada.Tipo() == Conector.ConexionType.TOP) filtro = infoSalida.salidaAbajo;

			return filtro.TipoEsAceptado(infoEntrada.tipoPropio);
		}
		public static bool CheckSectionType(Conector salida, Conector entrada)
        {
			//entrada debe de ser siempre un conector opuesto así que no hay que comprobar los dos
			if (!salida.EsHorizontal()) return true;

			var seccion = salida.ElBloque().GetSeccionPerteneciente();
			if (seccion == null) return true;
			return entrada.ElBloque().GetBlockData().seccionesAceptadas.TipoEsIgual(seccion);
        }

		public Conector GetConector(Conector.ConexionType type)
        {
            switch (type)
            {
                case Conector.ConexionType.TOP: return cT;
                case Conector.ConexionType.LEFT: return cL;
                case Conector.ConexionType.BOTTOM: return cB;
                case Conector.ConexionType.RIGHT: return cR;
				default: return null;
            }
        }

        #region Movimiento
        protected Vector3 GetMouseWorldPos()
		{
			Vector3 mousePoint = Input.mousePosition;
			mousePoint.z = mZCoord;
			return cam.ScreenToWorldPoint(mousePoint);
		}
		protected Vector3 GetPointOfCollision()
		{
			Vector3 p = GetMouseWorldPos() + mOffset;
			Vector3 dir = p - cam.transform.position;
			Ray r = new Ray(cam.transform.position,dir);

			if (Physics.Raycast(r, out RaycastHit info, maxDist, config.suelo.value))
			{
				Vector3 inv = cam.transform.position - info.point;
				float H = config.altura / (cam.transform.position.y - info.point.y);
				p = info.point + H * inv;
			}
			return p;
		}
		public Vector3 Constrain(Vector3 v)
		{
			if (useLimitBox)
			{
				if (!limitBox.Contains(v)) v = limitBox.ClosestPoint(v);
			}
			return v;
		}
		#endregion

		#region BlockRelationCalculations
		public BlockMovAndConexion GetBloqueAnterior()
		{
			if (cT != null && cT.EstaConectado()) return cT.ConectorConectado().ElBloque();
			else if (cL != null && cL.EstaConectado()) return cL.ConectorConectado().ElBloque();
			return null;
		}
		public BlockMovAndConexion GetBloqueIzquierdo()
        {
			if (cL != null && cL.EstaConectado()) return cL.ConectorConectado().ElBloque();
			return null;
		}
		public BlockMovAndConexion GetBloqueDerecho()
        {
			if (cR.EstaConectado()) return cR.ConectorConectado().ElBloque();
			return null;
        }

		public BlockType BuscarTipoSeccion()
        {
			BlockMovAndConexion bloqueSeccion = this;
			BlockMovAndConexion anterior = bloqueSeccion;
			int limit = 75;
			while (anterior != null && limit > 0)
			{
				bloqueSeccion = anterior;
				anterior = anterior.GetBloqueIzquierdo();
				limit--;
			}

			var bData = bloqueSeccion.GetBlockData();
			if(bData.tipoPropio.esSeccion) return seccionPresente = bData.tipoPropio;
			else return seccionPresente = null;
		}
		public BlockType GetSeccionPerteneciente() {
			if (!seccionPresente) return BuscarTipoSeccion();
			return seccionPresente; 
		}

		private Conector CalculateClosestConection(bool checkBlockType = true)
		{
			Conector res = null;
			float minD = float.PositiveInfinity;
			foreach( var pareja in Conector.distanciaEntradasTocando)
			{
				float d = pareja.Value;
				if (d >= minD) continue;
				var conector = pareja.Key;
				if (!conector.TieneCercanos()) continue;

				Conector elOtro = conector.TocandoCercano();
				//Conexiones de bloques que comparten chunks se ignoran
				if (BloqueRaizDeChunk(conector.ElBloque()) ==
					BloqueRaizDeChunk(elOtro.ElBloque())) continue;

				bool conectorEnChunk = FormaParteDelChunk(conector);
				bool otroEnChunk = FormaParteDelChunk(elOtro);
				if (conectorEnChunk && conector.EstaConectado()) continue;
				if (otroEnChunk && elOtro.EstaConectado()) continue;
				//Al menos uno de los conectores debe formar parte del bloque escogido
				if (!(conectorEnChunk || otroEnChunk)) continue;

				if (checkBlockType)
				{
					//Comprobar si salida acepta tipo de entrada
					if (!CheckConexionType(elOtro, conector)) continue;
					//Comprobar si bloque se va a unir a sección aceptada
					if (!CheckSectionType(elOtro, conector)) continue;
				}

				minD = d;
				res = conector;
			}
			return res;
		}
		private static BlockMovAndConexion BloqueRaizDeChunk(BlockMovAndConexion b)
		{
			int limit = 75;
			while(b != null && limit > 0)
			{
				var sup = b.GetBloqueAnterior();
				if (sup == null) return b;
				b = sup;

				limit--;
			}
			return null;
		}
		private bool FormaParteDelChunk(Conector c)
		{
			BlockMovAndConexion currentBlock = c.ElBloque();
			int limit = 75;
			while(currentBlock != null && limit > 0)
			{
				if (currentBlock == this) return true;
				currentBlock = currentBlock.GetBloqueAnterior();

				limit--;
			}
			return false;
		}
		public bool FormaParteDelChunk(BlockMovAndConexion block)
        {
			int limit = 75;
			while (block != null && limit > 0)
			{
				if (block == this) return true;
				block = block.GetBloqueAnterior();

				limit--;
			}
			return false;
		}

		public void FlotarOnConnectionChange(bool alsoCall = true)
        {
			if (alsoCall) onBlockConectionChanged?.Invoke();
			var b = GetBloqueIzquierdo();
			if (b != null) b.FlotarOnConnectionChange();
        }
		public void HundirOnConnectionChange( bool alsoCall = true)
		{
			if (alsoCall) onBlockConectionChanged?.Invoke();
			var b = GetBloqueDerecho();
			if (b != null) b.FlotarOnConnectionChange();
		}

		private Conector ConectorOpuestoA(Conector c)
		{
			var t = c.Tipo();
			switch (t) {
				case Conector.ConexionType.BOTTOM: return cT;
				case Conector.ConexionType.RIGHT: return cL;
				case Conector.ConexionType.TOP: return cB;
				case Conector.ConexionType.LEFT: return cR;
			}
			return null;
		}
		public bool AlMenosUnaEntradaConectada() { return cT.EstaConectado() || cL.EstaConectado(); }
		#endregion

		private bool OmitInteraction()
		{
			return cancelGrab;
		}

		public void UpdateVisuals()
        {
			if (TryGetComponent(out BlockModelControl bmc)) bmc.Activar(bd.TOP, bd.LEFT, bd.BOTTOM, bd.RIGHT);
		}

		#region MAIN
		private void Awake()
		{
			cam = Camera.main;
			trans = transform;

			coll = GetComponent<Collider>();
			audioS = GetComponent<AudioSource>();
			CQU = GetComponent<ContentQueryUnit>();
			UpdateVisuals();

			if(sendXAPIStatement) XAPI_Builder.CreateStatement_BlockAction(XAPI_Builder.BlockAction.GENERATED, bd.title);
		}

		private void OnMouseDown()
		{
			if (OmitInteraction()) return;
			//Preparado para acciones con ratón
			click = true;
			beginDrag = true;
			initMousePos = Input.mousePosition;
			iniPos = transform.position;
			oldHeight = trans.position.y;

			limitBox = new Bounds(config.centerLBox, config.sizeLBox);

			audioS.PlayOneShot(config.blockSelect);
		}

		private void OnMouseDrag()
		{
			if (OmitInteraction()) return;

			if (MouseMoved()) return;
			if (beginDrag) //First drag frame
			{
				oldHeight = trans.position.y;

				BeginDrag();
				beginDrag = false;
			}
			else
			{
				KeepDrag();
			}
		}

		public static void ColocarConexion(Conector entrada, Conector salida)
        {
			Transform bloqueAConectar = entrada.transform.parent;
			bloqueAConectar.SetParent(salida.transform.parent, false);
			bloqueAConectar.localPosition = salida.ElBloque().CalculatePositionForConectedBlock(salida, entrada);
		}

		private void OnMouseUp()
		{
			if (OmitInteraction()) return;

			trans.position = Constrain(trans.position);
			//if (oldHeight != trans.position.y) //Restablecer altura al soltar bloque
			//	trans.position = new Vector3(trans.position.x, oldHeight, trans.position.z);

			if (currentGhost != null) // Si hay fantasma, hay conexión disponible
			{
				Destroy(currentGhost);
				Conector cEntrada = CalculateClosestConection();
				if (cEntrada != null)
				{
					if (!cEntrada.TieneCercanos()) return;

					if (cEntrada.EstaConectado())
					{
						var antiguaSalida = cEntrada.AplastarConexionConCercano();
						var entradaOpuesta = this.ConectorOpuestoA(antiguaSalida);

						if (CheckConexionType(antiguaSalida, entradaOpuesta) &&
							CheckSectionType(antiguaSalida, entradaOpuesta))
                        {
							//Conecta el bloque de la conexion anterior en el lado opuesto de este bloque
							entradaOpuesta.ForzarConexion(antiguaSalida);
							ColocarConexion(entradaOpuesta, antiguaSalida);
						}
					}
					else
					{
						//Desconectar los otros conectores del bloque ya que sólo debe haber una entrada conectada a la vez
						if (cEntrada.ElBloque().AlMenosUnaEntradaConectada()) cEntrada.ElBloque().DisconectEntryConectors();
						cEntrada.AplicarConexion(true);
					}

					Conector cSalida = cEntrada.ConectorConectado();
					ColocarConexion(cEntrada, cSalida);

					//Invocar evento por todo el grupo
					FlotarOnConnectionChange();     //if (cEntrada.ElBloque() == this) cSalida.ElBloque().onBlockConectionChanged?.Invoke();
					HundirOnConnectionChange(false);//else cEntrada.ElBloque().onBlockConectionChanged?.Invoke();
					
					audioS.PlayOneShot(config.blockConect);
                    XAPI_Builder.CreateStatement_BlockAction(XAPI_Builder.BlockAction.CONNECTED, bd.title,cEntrada.ElBloque().bd.title);
                }
			}
			else if (wantToDeleteBlock)
            {
				Conector.distanciaEntradasTocando.Clear();
                XAPI_Builder.CreateStatement_BlockAction(XAPI_Builder.BlockAction.REMOVED, bd.title);
                Destroy(this.gameObject);
            }

			Conector.distanciaEntradasTocando.Clear();
			CollisionEnabling(true);
		}

        private void OnMouseOver()
        {
			var tooltip = TooltipManager.Instance;
			if (tooltip != null && bd != null)
            {
				tooltip.SetString(bd.name);
			}
        }
        #endregion

        private void OnDrawGizmosSelected()
		{
			//Dibuja la zona donde el bloque tiene permitido moverse.
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(config.centerLBox, config.sizeLBox);
		}
    }
}