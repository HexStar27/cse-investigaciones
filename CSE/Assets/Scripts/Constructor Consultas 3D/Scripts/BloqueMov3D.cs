using UnityEngine;
using Hexstar.CSE;

public class BloqueMov3D : MonoBehaviour
{
	[HideInInspector] public BloqueInfo3D info;
	[HideInInspector] public BloqueMov3D anterior;
	[HideInInspector] public BloqueMov3D siguiente;
	protected bool tieneAnterior;
	//Bloque3D esteBloque;

	protected Camera cam;
	protected Transform trans;
	protected Collider coll;
	protected Vector3 mOffset;
	protected float mZCoord;
	protected Vector3 initMPos;
	protected Vector3 iniPos;
	protected bool click = true;
	protected bool beginDrag = false;

	protected static float maxDist = 40;
	[SerializeField] protected LayerMask suelo;
	[SerializeField] protected LayerMask bloque;
	[SerializeField] protected LayerMask raiz;
	[SerializeField] protected LayerMask papelera;
	[SerializeField] protected float altura = 0.1f;
	[SerializeField] protected bool constrainX = false;
	[SerializeField] protected bool constrainY = false;
	[SerializeField] protected bool constrainZ = false;
	[SerializeField] protected Vector3 centerLBox;
	[SerializeField] protected Vector3 sizeLBox;
	[SerializeField] protected bool useLimitBox;
	[SerializeField] protected Vector3 offsetDeConexion;

	private Bounds limitBox;


	// Drag //
	private void BeginDrag()
	{
		DesconectarAnterior();

		mZCoord = cam.WorldToScreenPoint(trans.position).z;
		mOffset = trans.position - GetMouseWorldPos();
		iniPos = trans.position;

		PropagateColl(false);
	}
	private void KeepDrag()
	{
		trans.position = Constrain(GetPointOfCollision());
	}
	private bool MouseMoved()
	{
		click = click && initMPos == Input.mousePosition;
		return click;
	}
	public void DesconectarAnterior()
	{
		if (anterior != null)
		{
			anterior.siguiente = null;
			anterior = null;
		}
		tieneAnterior = false;
	}

	// On Mouse Up //
	private void AbrirSelector()
	{
		if(info != null) info.AbrirSelector();
	}

	protected virtual void BuscarAnterior()
	{
		tieneAnterior = false;

		//Crear rayo en dirección 'cámara a puntero'
		Vector3 p = GetMouseWorldPos() + mOffset;
		Vector3 dir = p - cam.transform.position;
		Ray r = new Ray(cam.transform.position,dir);

		if (Physics.Raycast(r, out RaycastHit info, maxDist, bloque.value | raiz.value | papelera.value))
		{
			int layerHit = 1 << info.collider.gameObject.layer;
			if (layerHit == papelera.value)
			{
				PropagateColl(true);
				if (anterior != null) DesconectarAnterior();
				if (siguiente != null) siguiente.DesconectarAnterior();
				Destroy(gameObject);
				return;
			}

			if(layerHit == raiz.value)
			{
				BloqueMov3D ultimo = info.collider.GetComponent<BloqueMov3D>();
				while(ultimo.siguiente != null && ultimo.siguiente != this)
				{
					ultimo = ultimo.siguiente;
				}
				ultimo.siguiente = this;
				anterior = ultimo;
			}
			else
			{
				BloqueMov3D aux = info.collider.GetComponent<BloqueMov3D>();

				//Para evitar el caso en el que dos bloques se retroalimentan y rompen el contínuo espacio tiempo
				if (aux == siguiente)
				{
					PropagateColl(true);
					return;
				}
				anterior = aux;

				if (anterior.siguiente != null) //El anterior ya tenía un siguiente
				{
					siguiente = anterior.siguiente;
					siguiente.anterior = this;
					anterior.siguiente = this;
				}
				else //El anterior no tiene siguiente
				{
					anterior.siguiente = this;
				}
			}
			tieneAnterior = true;
		}
		PropagateColl(true);
	}

	public void PropagateColl(bool value)
	{
		coll.enabled = value;
		if(siguiente != null) siguiente.PropagateColl(value);
	}

	// Movimiento //
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

		if (Physics.Raycast(r, out RaycastHit info, maxDist, suelo.value))
		{
			Vector3 inv = cam.transform.position - info.point;
			float H = altura / (cam.transform.position.y - info.point.y);
			p = info.point + H * inv;
		}
		return p;
	}
	public Vector3 Constrain(Vector3 v)
	{
		if (constrainX) v.x = iniPos.x;
		if (constrainY) v.y = iniPos.y;
		if (constrainZ) v.z = iniPos.z;

		if(useLimitBox)
		{
			if (!limitBox.Contains(v)) v = limitBox.ClosestPoint(v);
		}

		return v;
	}

	private bool OmitInteraction()
	{
		if (SelectorPalabras.instancia != null)
		{
			return SelectorPalabras.instancia.IsOpened();
		}
		return false;
	}


	private void Awake()
	{
		cam = Camera.main;
		trans = transform;
		coll = GetComponent<Collider>();
		info = GetComponent<BloqueInfo3D>();
	}

	private void FixedUpdate()
	{
		if(tieneAnterior)
		{
			trans.position = anterior.transform.position + offsetDeConexion;
		}
	}

	private void OnMouseDown()
	{
		if (OmitInteraction()) return;

		click = true;
		beginDrag = true;
		initMPos = Input.mousePosition;
		iniPos = transform.position;

		limitBox = new Bounds(centerLBox, sizeLBox);
	}

	private void OnMouseDrag()
	{
		if (OmitInteraction()) return;

		MouseMoved();
		if(!click)
		{
			if (beginDrag)
			{
				BeginDrag();
				beginDrag = false;
			}
			else
			{
				KeepDrag();
			}
		}
	}

	private void OnMouseUp()
	{
		if (OmitInteraction()) return;

		if (click) AbrirSelector();
		else BuscarAnterior();	
	}
}
