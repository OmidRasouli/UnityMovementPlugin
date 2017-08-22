using UnityEngine;

public enum State
{
    TwoPoint, Plygon, Circle
}
public class ScrMoveInPathHelper : MonoBehaviour
{
    #region Instance
    public void Awake()
    {
        TwoPonit = new TwoPointClass();
        MultiPonit = new MultiPointClass();
        CircularPath = new CircularPathClass();
        SpiralPath = new SpiralPathClass();
    }
    #endregion

    #region SubClasses
    public class TwoPointClass
    {
        #region Vals
        protected Transform TargetTransform;
        protected Vector3 ThisPoint;
        protected Vector3 TargetPoint;
        protected Vector3 Distance;
        protected float Speed;
        protected bool Loop;
        protected bool LoopFlag = true;
        protected bool LinearOrEase;
        #endregion

        /// <summary>
        /// Initialize your parameters to transform between two points
        /// </summary>
        /// <param name="Transform">Gameobject transform as Transfrom</param>
        /// <param name="FirstPos">First point as Vector3</param>
        /// <param name="SecondPos">Second Point as Vector3</param>
        /// <param name="Speed">Speed as float</param>
        /// <param name="Loop">Loop as boolean</param>
        /// <param name="Linear">Linear or Ease-in-out as boolean</param>
        public void Init(Transform Transform, Vector3 FirstPos, Vector3 SecondPos, float Speed, bool Loop, bool Linear)
        {
            TargetTransform = Transform;
            ThisPoint = FirstPos;
            TargetPoint = SecondPos;
            this.Speed = Speed;
            this.Loop = Loop;
            Distance = TargetPoint - ThisPoint;
            Transform.position = ThisPoint;
            LinearOrEase = Linear;

            if (!LinearOrEase)
                ThisPoint = ThisPoint + (TargetPoint - ThisPoint) / 2;
        }
        public void Update()
        {
            if (LoopFlag)
            {
                if (LinearOrEase)
                {
                    linear();
                }
                else
                {
                    ease();
                }
            }
        }
        private void ease()
        {
            if (Mathf.Sin(Time.time * Speed) > 0)
            {
                TargetTransform.position = Vector3.Lerp(ThisPoint, TargetPoint, Mathf.Abs(Mathf.Sin(Time.time * Speed)) * 1.0f);
            }
            else
            {
                TargetTransform.position = Vector3.Lerp(ThisPoint, ThisPoint - (TargetPoint - ThisPoint), Mathf.Abs(Mathf.Sin(Time.time * Speed)) * 1.0f);
            }
            if (Vector3.Distance(TargetTransform.position, TargetPoint) < 0.5f || Vector3.Distance(TargetTransform.position, ThisPoint) > (Vector3.Distance(ThisPoint, TargetPoint) + 0.5f))
            {
                LoopFlag = Loop;
            }
        }
        private void linear()
        {
            TargetTransform.position = Vector3.Lerp(ThisPoint, TargetPoint, Mathf.PingPong(Time.time * Speed, 1.0f));
            if (Vector3.Distance(TargetTransform.position, TargetPoint) < 0.1f || Vector3.Distance(TargetTransform.position, ThisPoint) > (Vector3.Distance(ThisPoint, TargetPoint) + 0.1f))
            {
                Speed *= -1;
                LoopFlag = Loop;
            }
        }
    }
    public TwoPointClass TwoPonit;

    public class MultiPointClass
    {
        #region Vals
        protected Transform TargetTransform;
        protected Vector3[] Points;
        protected Vector3 ThisPoint;
        protected Vector3 TargetPoint;
        protected Vector3 Direction;
        protected int PointsLength;
        protected float Devide = 100;
        protected float Speed;
        protected int Counter = 0;
        protected int CounterTemp = 1;
        protected bool Cycle;
        protected bool CycleFlag = false;
        #endregion
        ///<summary>
        /// Initialize your parameters to transform between many points
        /// </summary>
        /// <param name="Transform">Gameobject transform as Transfrom</param>
        /// <param name="Points">Points of polygon as vector3 array</param>
        /// <param name="Speed">Speed as float</param>
        /// <param name="Cycle">Cycle path or backward path as boolean</param>
        public void Init(Transform Transform, Vector3[] Points, float Speed, bool Cycle)
        {
            TargetTransform = Transform;
            this.Points = Points;
            this.Speed = Speed;
            ThisPoint = Points[Counter];
            TargetPoint = Points[Counter + 1];
            PointsLength = Points.Length;
            Direction = (TargetPoint - ThisPoint);
            TargetTransform.position = ThisPoint;
            this.Cycle = Cycle;
        }
        public void Update()
        {
            if (Cycle)
            {
                MoveInPathCycle();
            }
            else
            {
                MoveInPathBack();
            }
        }
        private void MoveInPathCycle()
        {
            TargetTransform.position += Direction * Speed / Devide;
            if (Vector3.Distance(TargetTransform.position, TargetPoint) < 0.1f)
            {
                Counter++;
                CounterTemp++;
                if (Counter == PointsLength)
                {
                    Counter = 0;
                }
                if (CounterTemp >= PointsLength)
                {
                    CounterTemp = 0;
                }
                ThisPoint = Points[Counter];
                TargetPoint = Points[CounterTemp];
                Direction = (TargetPoint - ThisPoint);
            }
        }
        private void MoveInPathBack()
        {
            TargetTransform.position += Direction * Speed / Devide;
            if (Vector3.Distance(TargetTransform.position, TargetPoint) < 0.1f)
            {
                if (CycleFlag)
                {
                    Counter--;
                    CounterTemp--;
                    if (Counter == 0)
                    {
                        Counter = 0;
                    }
                    if (CounterTemp < 0)
                    {
                        CounterTemp = 1;
                        CycleFlag = !CycleFlag;
                    }
                }
                else
                {
                    Counter++;
                    CounterTemp++;
                    if (Counter == PointsLength)
                    {
                        Counter = PointsLength - 1;
                    }
                    if (CounterTemp >= PointsLength)
                    {
                        CounterTemp = PointsLength - 2;
                        CycleFlag = !CycleFlag;
                    }
                }
                ThisPoint = Points[Counter];
                TargetPoint = Points[CounterTemp];
                Direction = (TargetPoint - ThisPoint);
            }
        }
    }
    public MultiPointClass MultiPonit;

    public class CircularPathClass
    {
        #region Vals
        protected Transform TargetTransform;
        protected Vector3 Position = Vector3.zero;
        protected float Angle = 0;
        protected float Radius = 1;
        protected float OriginX = 0;
        protected float OriginY = 0;
        protected float OriginZ = 0;
        protected float Speed;
        protected bool RightToLeft = false;
        protected float Direction = 1;
        protected bool isArc = false;
        protected bool Loop = true;
        protected bool LoopFlag = true;
        protected float ArcLength;
        protected int WhichVectors = 0;
        #endregion

        #region Init 1
        /// <summary>
        /// Initialize circular path to transform
        /// </summary>
        /// <param name="Transform">Gameobject transform as Transfrom</param>
        /// <param name="WhichVectors">0: X and Y, 1: X and Z, 2: Y and Z as integer</param>
        /// <param name="Position">Middle of axis as vector3</param>
        /// <param name="Angle">Angle as float</param>
        /// <param name="Radius">Radius as float</param>
        /// <param name="Speed">Speed as float</param>
        /// <param name="Loop">Loop as boolean</param>
        public void Init(Transform Transform, int WhichVectors, Vector3 Position, float Angle, float Radius, float Speed, bool Loop)
        {
            ValueInitialize(Transform, WhichVectors, Position, Angle, Radius, Speed, Loop);
        }
        #endregion

        #region Init 2
        /// <summary>
        /// Initialize circular path to transform
        /// </summary>
        /// <param name="Transform">Gameobject transform as Transfrom</param>
        /// <param name="WhichVectors">0: X and Y, 1: X and Z, 2: Y and Z as integer</param>
        /// <param name="Position">Middle of axis as vector3</param>
        /// <param name="Angle">Angle as float</param>
        /// <param name="Radius">Radius as float</param>
        /// <param name="Speed">Speed as float</param>
        /// <param name="Loop">Loop as boolean</param>
        /// <param name="RightToLeft">Direction of movement as boolean</param>
        public void Init(Transform Transform, int WhichVectors, Vector3 Position, float Angle, float Radius, float Speed, bool Loop, bool RightToLeft)
        {
            ValueInitialize(Transform, WhichVectors, Position, Angle, Radius, Speed, Loop);
            Direction = (RightToLeft) ? -1 : 1;
        }
        #endregion

        #region Init 3
        /// <summary>
        /// Initialize circular path to transform
        /// </summary>
        /// <param name="Transform">Gameobject transform as Transfrom</param>
        /// <param name="WhichVectors">0: X and Y, 1: X and Z, 2: Y and Z as integer</param>
        /// <param name="Position">Middle of axis as vector3</param>
        /// <param name="Angle">Angle as float</param>
        /// <param name="Radius">Radius as float</param>
        /// <param name="Speed">Speed as float</param>
        /// <param name="Loop">Loop as boolean</param>
        /// <param name="RightToLeft">Direction of movement as boolea</param>
        /// <param name="ArcLength">If you want move in arc path define length as float</param>
        public void Init(Transform Transform, int WhichVectors, Vector3 Position, float Angle, float Radius, float Speed, bool Loop, bool RightToLeft, float ArcLength)
        {
            ValueInitialize(Transform, WhichVectors, Position, Angle, Radius, Speed, Loop);
            Direction = (RightToLeft) ? -1 : 1;
            ArcLength *= Mathf.PI / 180;
            this.ArcLength = ArcLength;
            isArc = true;
        }
        #endregion

        #region Initialize common values
        private void ValueInitialize(Transform Transform, int WhichVectors, Vector3 Position, float Angle, float Radius, float Speed, bool Loop)
        {
            TargetTransform = Transform;
            this.WhichVectors = WhichVectors % 3;
            OriginX = Transform.localScale.x / 2;
            OriginY = Transform.localScale.y / 2;
            OriginZ = Transform.localScale.z / 2;
            this.Position = Position;
            this.Angle = Angle;
            this.Radius = Radius;
            this.Speed = Speed;
            this.Loop = Loop;
        }
        #endregion

        #region Update
        public void Update()
        {
            switch (WhichVectors)
            {
                case 0:
                    XY();
                    break;
                case 1:
                    XZ();
                    break;
                case 2:
                    YZ();
                    break;
            }
        }
        #endregion

        #region Axis Methods
        #region XY
        private void XY()
        {
            DefaultValues();
            OriginX = Mathf.Cos(Angle * Direction) * Radius + Position.x;
            OriginY = Mathf.Sin(Angle * Direction) * Radius + Position.y;
            TargetTransform.position = new Vector3(OriginX, OriginY, TargetTransform.position.z);
        }
        #endregion

        #region XZ
        private void XZ()
        {
            OriginX = Mathf.Cos(Angle * Direction) * Radius * Direction + Position.x;
            OriginZ = Mathf.Sin(Angle * Direction) * Radius * Direction + Position.z;
            DefaultValues();
            TargetTransform.position = new Vector3(OriginX, TargetTransform.position.y, OriginZ);
        }
        #endregion

        #region YZ
        private void YZ()
        {
            OriginZ = Mathf.Cos(Angle * Direction) * Radius * Direction + Position.z;
            OriginY = Mathf.Sin(Angle * Direction) * Radius * Direction + Position.y;
            DefaultValues();
            TargetTransform.position = new Vector3(TargetTransform.position.x, OriginY, OriginZ);
        }
        #endregion

        #region Axis default values
        private void DefaultValues()
        {
            if (LoopFlag)
            {
                if (Angle >= Mathf.PI * 2f)
                {
                    LoopFlag = Loop;
                    Angle = 0;
                }
                Angle += Speed * Time.deltaTime;
                if (isArc)
                {
                    if (Angle > ArcLength)
                    {
                        Speed *= -1;
                    }
                    if (Angle <= 0)
                    {
                        Speed *= -1;
                    }
                }
            }
        }
        #endregion
        #endregion
    }
    public CircularPathClass CircularPath;

    public class SpiralPathClass
    {
        #region Vals
        protected Transform TargetTransform;
        protected Vector3 Position = Vector3.zero;
        protected float Angle = 0;
        protected float Radius = 1;
        protected float OriginX = 0;
        protected float OriginY = 0;
        protected float OriginZ = 0;
        protected float Speed;
        protected bool RightToLeft = false;
        protected float Direction = 1;
        protected bool isArc = false;
        protected bool Loop = true;
        protected bool LoopFlag = true;
        protected bool SpiralFlag = true;
        protected float ArcLength;
        protected float Timer = 0;
        protected float RadiusCounter = 0;
        protected int WhichVectors = 0;
        #endregion

        #region Init
        /// <summary>
        /// Initialize circular path to transform
        /// </summary>
        /// <param name="Transform">Gameobject transform as Transfrom</param>
        /// <param name="WhichVectors">0: X and Y, 1: X and Z, 2: Y and Z as integer</param>
        /// <param name="Position">Middle of axis as vector3</param>
        /// <param name="Angle">Angle as float</param>
        /// <param name="Radius">Radius as float</param>
        /// <param name="Speed">Speed as float</param>
        /// <param name="Loop">Loop as boolean</param>
        public void Init(Transform Transform, int WhichVectors, Vector3 Position, float Angle, float Radius, float Speed, bool Loop)
        {
            ValueInitialize(Transform, WhichVectors, Position, Angle, Radius, Speed, Loop);
        }
        #endregion

        #region Initialize common values
        private void ValueInitialize(Transform Transform, int WhichVectors, Vector3 Position, float Angle, float Radius, float Speed, bool Loop)
        {
            TargetTransform = Transform;
            this.WhichVectors = WhichVectors % 3;
            OriginX = Transform.localScale.x / 2;
            OriginY = Transform.localScale.y / 2;
            OriginZ = Transform.localScale.z / 2;
            this.Position = Position;
            this.Angle = Angle;
            this.Radius = Radius;
            this.Speed = Speed;
            this.Loop = Loop;
        }
        #endregion

        #region Update
        public void Update()
        {
            switch (WhichVectors)
            {
                case 0:
                    XY();
                    break;
                case 1:
                    XZ();
                    break;
                case 2:
                    YZ();
                    break;
            }
        }
        #endregion

        #region Axis Methods
        #region XY
        private void XY()
        {
            OriginX = Mathf.Cos(Angle) * RadiusCounter + Position.x * Direction;
            OriginY = Mathf.Sin(Angle) * RadiusCounter + Position.y * Direction;
            DefaultValues();
            TargetTransform.position = new Vector3(OriginX, OriginY, TargetTransform.position.z);
        }
        #endregion

        #region XZ
        private void XZ()
        {
            OriginX = Mathf.Cos(Angle) * RadiusCounter + Position.x * Direction;
            OriginZ = Mathf.Sin(Angle) * RadiusCounter + Position.z * Direction;
            DefaultValues();
            TargetTransform.position = new Vector3(OriginX, TargetTransform.position.y, OriginZ);
        }
        #endregion

        #region YZ
        private void YZ()
        {
            OriginZ = Mathf.Cos(Angle) * RadiusCounter + Position.z * Direction;
            OriginY = Mathf.Sin(Angle) * RadiusCounter + Position.y * Direction;
            DefaultValues();
            TargetTransform.position = new Vector3(TargetTransform.position.x, OriginY, OriginZ);
        }
        #endregion

        #region Axis default values
        private void DefaultValues()
        {
            if (LoopFlag)
            {
                if (RadiusCounter >= Radius && SpiralFlag)
                {
                    SpiralFlag = false;
                    Timer = Time.time;
                }
                if (RadiusCounter <= 0)
                {
                    SpiralFlag = true;
                    //Timer = Time.time;
                }
                if (SpiralFlag)
                {
                    RadiusCounter += Mathf.Cos(Speed * Time.time) * Time.deltaTime + 0.01f;
                }
                else
                {
                    RadiusCounter += Mathf.Cos(Speed * Time.time) * Time.deltaTime - 0.01f;
                }
                if (Angle >= Mathf.PI * 2f)
                {
                    LoopFlag = Loop;
                    Angle = 0;
                }
                Angle += Speed * Time.deltaTime * Direction;
                if (isArc)
                {
                    if (Angle >= ArcLength)
                    {
                        Direction = -1;
                    }
                    if (Angle <= 0)
                    {
                        Direction = +1;
                    }
                }
            }
        }
        #endregion
        #endregion
    }
    public SpiralPathClass SpiralPath;
    #endregion

    #region Values
    [HideInInspector]
    public State Paths = State.TwoPoint;
    #endregion

    #region Start
    void Start()
    {
        switch (Paths)
        {
            case State.TwoPoint:
                break;
            case State.Plygon:
                break;
            case State.Circle:
                break;
            default:
                break;
        }
    }
    #endregion
}
