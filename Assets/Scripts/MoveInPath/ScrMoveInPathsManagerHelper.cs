#region Namespaces
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

[RequireComponent(typeof(ScrMoveInPathHelper))]
public class ScrMoveInPathsManagerHelper : MonoBehaviour
{
    public enum Paths
    {
        TwoPoints,
        Polygon,
        Circular,
        Spiral
    }
    public enum WhichVector
    {
        Z,
        Y,
        X
    }

    #region Public values
    [HideInInspector]
    public Transform Tr;
    public bool UseInBehavior;
    [HideInInspector]
    public Paths PathsEnum;
    [HideInInspector]
    public float Speed;
    [HideInInspector]
    public bool Loop;
    [HideInInspector]
    public ScrMoveInPathHelper ScrMoveInPathHelperClassClone;
    #endregion

    #region Define two points path values
    [System.Serializable]
    public class TwoPointsValuesClass
    {
        [HideInInspector]
        public Vector3[] Points;
        [HideInInspector]
        public bool Linear;
    }

    [HideInInspector]
    public TwoPointsValuesClass TwoPointClassClone;
    #endregion

    #region Define polygon path values
    [System.Serializable]
    public class PolygonValuesClass
    {
        [HideInInspector]
        public Vector3[] Points;
        public bool Cycle;
    }
    [HideInInspector]
    public PolygonValuesClass PolygonClassClone = new PolygonValuesClass();
    #endregion

    #region Define circular path values
    [System.Serializable]
    public class CircularValuesClass
    {
        [HideInInspector]
        public Vector3 CenterPosition;
        [HideInInspector]
        public float Radius;
        [HideInInspector]
        public float Angle;
        [HideInInspector]
        public bool Reverse;
        [HideInInspector]
        public float ArcLength;
        [HideInInspector]
        public bool IsArc;
        [HideInInspector]
        public WhichVector WhichVectorEnum;
        [HideInInspector]
        public int WhichVectorInt;
    }
    [HideInInspector]
    public CircularValuesClass CircularClassClone = new CircularValuesClass();
    #endregion

    #region Define spiral path values
    [System.Serializable]
    public class SpiralValuesClass
    {
        [HideInInspector]
        public Vector3 CenterPosition;
        [HideInInspector]
        public float Radius;
        [HideInInspector]
        public float Angle;
        [HideInInspector]
        public bool Reverse;
        [HideInInspector]
        public WhichVector WhichVectorEnum;
        [HideInInspector]
        public int WhichVectorInt;
    }
    [HideInInspector]
    public SpiralValuesClass SpiralClassClone = new SpiralValuesClass();
    #endregion

    void Awake()
    {
        if (Tr == null)
            Tr = transform;
        ScrMoveInPathHelperClassClone = this.GetComponent<ScrMoveInPathHelper>();
        PathsMethod();

    }
    void Update()
    {
        switch (PathsEnum)
        {
            case Paths.TwoPoints:
                ScrMoveInPathHelperClassClone.TwoPonit.Update();
                break;
            case Paths.Polygon:
                ScrMoveInPathHelperClassClone.MultiPonit.Update();
                break;
            case Paths.Circular:
                ScrMoveInPathHelperClassClone.CircularPath.Update();
                break;
            case Paths.Spiral:
                ScrMoveInPathHelperClassClone.SpiralPath.Update();
                break;
        }
    }

    #region Path Method
    void PathsMethod()
    {
        switch (PathsEnum)
        {
            case Paths.TwoPoints:
                TwoPointMethod();
                break;
            case Paths.Polygon:
                PolygonMethod();
                break;
            case Paths.Circular:
                CircularMethod();
                break;
            case Paths.Spiral:
                SpiralMethod();
                break;
        }
    }
    #endregion

    protected void TwoPointMethod()
    {
        ScrMoveInPathHelperClassClone.TwoPonit.Init(Tr, TwoPointClassClone.Points[0], TwoPointClassClone.Points[1], Speed, Loop, TwoPointClassClone.Linear);
    }
    protected void PolygonMethod()
    {
        ScrMoveInPathHelperClassClone.MultiPonit.Init(Tr, PolygonClassClone.Points, Speed, PolygonClassClone.Cycle);
    }
    protected void CircularMethod()
    {
        if (!(CircularClassClone.Reverse && CircularClassClone.IsArc))
            ScrMoveInPathHelperClassClone.CircularPath.Init(Tr, CircularClassClone.WhichVectorInt, CircularClassClone.CenterPosition, CircularClassClone.Angle, CircularClassClone.Radius, Speed, Loop);

        if (CircularClassClone.Reverse && !CircularClassClone.IsArc)
            ScrMoveInPathHelperClassClone.CircularPath.Init(Tr, CircularClassClone.WhichVectorInt, CircularClassClone.CenterPosition, CircularClassClone.Angle, CircularClassClone.Radius, Speed, Loop, CircularClassClone.Reverse);

        if (CircularClassClone.IsArc)
            ScrMoveInPathHelperClassClone.CircularPath.Init(Tr, CircularClassClone.WhichVectorInt, CircularClassClone.CenterPosition, CircularClassClone.Angle, CircularClassClone.Radius, Speed, Loop, CircularClassClone.Reverse, CircularClassClone.ArcLength);

    }
    protected void SpiralMethod()
    {
        ScrMoveInPathHelperClassClone.SpiralPath.Init(Tr, SpiralClassClone.WhichVectorInt, SpiralClassClone.CenterPosition, SpiralClassClone.Angle, SpiralClassClone.Radius, Speed, Loop);
    }
}


#region Editor
#if UNITY_EDITOR
[CustomEditor(typeof(ScrMoveInPathsManagerHelper))]
public class ScrMoveInPathsEditorHelper : Editor
{
    #region Define Value
    protected ScrMoveInPathsManagerHelper thisPath;
    //protected bool IsArc;

    #endregion

    #region Editor
    public override void OnInspectorGUI()
    {
        thisPath = target as ScrMoveInPathsManagerHelper;

        base.OnInspectorGUI();

        if (thisPath.UseInBehavior)
        {
            #region Define public Values

            EditorGUILayout.Space();

            thisPath.PathsEnum = (ScrMoveInPathsManagerHelper.Paths)EditorGUILayout.EnumPopup(new GUIContent("Movement methods", "Movement methods\nTwo point: Move between two point\nPolygone movement: Move between multiple points\nCircular: Move in circular path\nSpiral: Move in spiral path"), thisPath.PathsEnum);

            EditorGUILayout.Space();

            thisPath.Tr = EditorGUILayout.ObjectField(new GUIContent("Target transfom: ", "Select gameobject transform"), thisPath.Tr, typeof(Transform), true) as Transform;

            if (thisPath.Speed <= 0)
                thisPath.Speed = 0.1f;

            thisPath.Speed = EditorGUILayout.FloatField(new GUIContent("Speed: ", "Speed of movement"), thisPath.Speed);

            thisPath.Loop = EditorGUILayout.Toggle(new GUIContent("Loop: ", "Loop movement"), thisPath.Loop);
            EditorGUILayout.Space();
            #endregion
            #region Select Paths Method
            switch (thisPath.PathsEnum)
            {
                #region Two points path
                case ScrMoveInPathsManagerHelper.Paths.TwoPoints:

                    if (thisPath.TwoPointClassClone == null || thisPath.TwoPointClassClone.Points == null || thisPath.TwoPointClassClone.Points.Length != 2)
                    {
                        thisPath.TwoPointClassClone = new ScrMoveInPathsManagerHelper.TwoPointsValuesClass();
                        thisPath.TwoPointClassClone.Points = new Vector3[2];
                    }
                    thisPath.TwoPointClassClone.Points[0] = EditorGUILayout.Vector3Field(new GUIContent("Point " + 1 + ": ", "First point"), thisPath.TwoPointClassClone.Points[0]);
                    thisPath.TwoPointClassClone.Points[1] = EditorGUILayout.Vector3Field(new GUIContent("Point " + 2 + ": ", "Second point"), thisPath.TwoPointClassClone.Points[1]);
                    thisPath.TwoPointClassClone.Linear = EditorGUILayout.Toggle(new GUIContent("Linear: ", "Linear animate or ease-in-out\nActive: Linear\nDeactive: ease-in-out"), thisPath.TwoPointClassClone.Linear);
                    break;
                #endregion

                #region Polygon path
                case ScrMoveInPathsManagerHelper.Paths.Polygon:
                    if (thisPath.PolygonClassClone.Points == null || thisPath.PolygonClassClone.Points.Length < 3)
                        thisPath.PolygonClassClone.Points = new Vector3[3];

                    var length = EditorGUILayout.IntField(new GUIContent("Length Points : ", "How many points do you have?\nMinimum number of points are 3"), thisPath.PolygonClassClone.Points.Length);

                    if (thisPath.PolygonClassClone.Points.Length != length)
                        thisPath.PolygonClassClone.Points = new Vector3[length];

                    for (int i = 0; i < thisPath.PolygonClassClone.Points.Length; i++)
                        thisPath.PolygonClassClone.Points[i] = EditorGUILayout.Vector3Field("Point " + (i + 1) + ": ", thisPath.PolygonClassClone.Points[i]);

                    EditorGUILayout.Space();

                    thisPath.PolygonClassClone.Cycle = EditorGUILayout.Toggle(new GUIContent("Cycle: ", "Continue the path or move back"), thisPath.PolygonClassClone.Cycle);
                    break;
                #endregion

                #region Circular path
                case ScrMoveInPathsManagerHelper.Paths.Circular:
                    thisPath.CircularClassClone.WhichVectorEnum = (ScrMoveInPathsManagerHelper.WhichVector)EditorGUILayout.EnumPopup(new GUIContent("Which Vector? ", "Select your axis for rotation\nRotate over the axis"), thisPath.CircularClassClone.WhichVectorEnum);
                    switch (thisPath.CircularClassClone.WhichVectorEnum)
                    {
                        case ScrMoveInPathsManagerHelper.WhichVector.Z:
                            thisPath.CircularClassClone.WhichVectorInt = 0;
                            break;
                        case ScrMoveInPathsManagerHelper.WhichVector.Y:
                            thisPath.CircularClassClone.WhichVectorInt = 1;
                            break;
                        case ScrMoveInPathsManagerHelper.WhichVector.X:
                            thisPath.CircularClassClone.WhichVectorInt = 2;
                            break;
                    }

                    EditorGUILayout.Space();

                    thisPath.CircularClassClone.CenterPosition = EditorGUILayout.Vector3Field(new GUIContent("Center Point: ", "Define center point"), thisPath.CircularClassClone.CenterPosition);
                    thisPath.CircularClassClone.Angle = EditorGUILayout.FloatField(new GUIContent("Angle: ", "Angle of start"), thisPath.CircularClassClone.Angle);
                    thisPath.CircularClassClone.Radius = EditorGUILayout.FloatField(new GUIContent("Radius: ", "Distance from center"), thisPath.CircularClassClone.Radius);

                    EditorGUILayout.Space();

                    thisPath.CircularClassClone.Reverse = EditorGUILayout.Toggle(new GUIContent("Reverse: ", "Move left to right"), thisPath.CircularClassClone.Reverse);

                    EditorGUILayout.Space();

                    thisPath.CircularClassClone.IsArc = EditorGUILayout.Toggle(new GUIContent("Is Arc?", "Is it arc?"), thisPath.CircularClassClone.IsArc);
                    //thisPath.CircularClassClone.IsArc = IsArc;
                    if (thisPath.CircularClassClone.IsArc)
                        thisPath.CircularClassClone.ArcLength = EditorGUILayout.FloatField(new GUIContent("Arc Length: ", "Length of arc\nIt will move in this arc"), thisPath.CircularClassClone.ArcLength);
                    break;
                #endregion

                #region Spiral path
                case ScrMoveInPathsManagerHelper.Paths.Spiral:
                    thisPath.SpiralClassClone.WhichVectorEnum = (ScrMoveInPathsManagerHelper.WhichVector)EditorGUILayout.EnumPopup(new GUIContent("Which Vector? ", "Select your axis for rotation\nRotate over the axis"), thisPath.SpiralClassClone.WhichVectorEnum);
                    switch (thisPath.SpiralClassClone.WhichVectorEnum)
                    {
                        case ScrMoveInPathsManagerHelper.WhichVector.Z:
                            thisPath.SpiralClassClone.WhichVectorInt = 0;
                            break;
                        case ScrMoveInPathsManagerHelper.WhichVector.Y:
                            thisPath.SpiralClassClone.WhichVectorInt = 1;
                            break;
                        case ScrMoveInPathsManagerHelper.WhichVector.X:
                            thisPath.SpiralClassClone.WhichVectorInt = 2;
                            break;
                    }

                    EditorGUILayout.Space();

                    thisPath.SpiralClassClone.CenterPosition = EditorGUILayout.Vector3Field(new GUIContent("Center Point: ", "Define center point"), thisPath.SpiralClassClone.CenterPosition);
                    thisPath.SpiralClassClone.Angle = EditorGUILayout.FloatField(new GUIContent("Angle: ", "Angle of start"), thisPath.SpiralClassClone.Angle);
                    thisPath.SpiralClassClone.Radius = EditorGUILayout.FloatField(new GUIContent("Radius: ", "Distance from center"), thisPath.SpiralClassClone.Radius);

                    EditorGUILayout.Space();

                    thisPath.SpiralClassClone.Reverse = EditorGUILayout.Toggle(new GUIContent("Reverse: ", "Move left to right"), thisPath.SpiralClassClone.Reverse);
                    break;
                #endregion

                #region Default path
                default:

                    if (thisPath.PolygonClassClone == null || thisPath.PolygonClassClone.Points == null || thisPath.PolygonClassClone.Points.Length != 2)
                    {
                        thisPath.TwoPointClassClone = new ScrMoveInPathsManagerHelper.TwoPointsValuesClass();
                        thisPath.TwoPointClassClone.Points = new Vector3[2];
                    }

                    thisPath.TwoPointClassClone.Points[0] = EditorGUILayout.Vector3Field(new GUIContent("Point " + 1 + ": ", "First point"), thisPath.TwoPointClassClone.Points[0]);
                    thisPath.TwoPointClassClone.Points[1] = EditorGUILayout.Vector3Field(new GUIContent("Point " + 2 + ": ", "Second point"), thisPath.TwoPointClassClone.Points[1]);
                    break;
                    #endregion
            }
            #endregion
        }
    }
    #endregion
}
#endif
#endregion