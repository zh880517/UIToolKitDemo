using UnityEngine;

public class AimTarget : MonoBehaviour
{
    public Transform Child;
    public Transform Target;
    public Quaternion CustomRotation;
    public float Angle;
    public bool Rotate;

    private void OnDrawGizmos()
    {
        if (!Child || !Target)
            return;
        Quaternion rootR = transform.rotation;
        Vector3 rootP  = transform.position;
        Quaternion childLocalR = Child.localRotation;
        Vector3 childLocalP = Child.localPosition;
        Draw(rootR, childLocalR, childLocalP, Color.green);

        Vector3 targetToRoot = Target.position - rootP;
        //1、让根节点朝向目标
        Quaternion rootFaceRotation = Quaternion.LookRotation(targetToRoot, Vector3.up);
        Draw(rootFaceRotation, childLocalR, childLocalP, Color.red);
        Quaternion rootChildRotation = rootFaceRotation * Quaternion.Inverse(childLocalR);
        Draw(rootChildRotation, childLocalR, childLocalP, Color.blue);
        Vector3 newChildPos = rootP + rootChildRotation * childLocalP;
        //对角点
        Vector3 diagonalP = newChildPos + targetToRoot;
        DrawPoint(diagonalP, Color.blue);
        Vector3 dToR = diagonalP - rootP;
        float angle = Vector3.Angle(dToR, targetToRoot);
        Vector3 axis = Vector3.Cross(dToR, targetToRoot);
        axis.Normalize();
        Draw(rootChildRotation * Quaternion.AngleAxis(Angle, axis), childLocalR, childLocalP, Color.black);
        DrawLine(transform.position, transform.position + axis, Color.magenta);
        Quaternion newRotation = rootChildRotation * Quaternion.AngleAxis(angle, axis);
        //Draw(rootChildRotation * CustomRotation, childLocalR, childLocalP, Color.black);
        //Quaternion newRotation = rootChildRotation * CustomRotation;
        //Quaternion newRotation = rootChildRotation * Quaternion.FromToRotation(dToR, targetToRoot);
        //Draw(newRotation, childLocalR, childLocalP, Color.cyan);
        if (Rotate)
        {
            transform.rotation = newRotation;
        }
    }

    private void Draw(Quaternion rootR, Quaternion childLocalR, Vector3 childLocalP, Color color)
    {
        Vector3 rootP = transform.position;
        Vector3 childWorldP = rootP + rootR * childLocalP;
        DrawPoint(childWorldP, color);//子节点位置
        DrawLine(rootP, childWorldP, color);//根节点->子节点
        DrawLine(transform.position, rootR, 1, color);//根节点朝向
        float length  = (Target.position - transform.position).magnitude;
        DrawLine(transform.position, rootR * childLocalR, 1, color);//根节点的子节点朝向
        DrawLine(childWorldP, rootR * childLocalR, length, color);//子节点朝向
    }

    private static void DrawLine(Vector3 start, Quaternion rotation, float length, Color color)
    {
        Color old = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawLine(start, start + rotation * new Vector3(0, 0, length));
        Gizmos.color = old;
    }

    private static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        Color old = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawLine(start, end);
        Gizmos.color = old;
    }

    private static void DrawPoint(Vector3 pos, Color color)
    {
        Color old = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(pos, 0.1f);
        Gizmos.color = old;
    }
}
