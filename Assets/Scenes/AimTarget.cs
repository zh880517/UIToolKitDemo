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
        /*    V-C-----D
         *    |/     /
         *    R-----T
         *    R为根节点，C为子节点，T为目标点，D为R的对焦点，V为在CD上的投影点
         *    保持T点不变，旋转R，使点T在直线CD上
         *    旋转后VR依然垂直于CD，VRT三点组成一个直角三角形，可计算出旋转后RV和RT的夹角θ
         *    90-θ就得到旋转需要的角度
         */
        float angleC = Vector3.Angle(diagonalP - newChildPos, newChildPos - rootP);
        float lenghth = (newChildPos - rootP).magnitude * Mathf.Sin(angleC * Mathf.Deg2Rad);
        float angle2 = Mathf.Acos(lenghth / targetToRoot.magnitude) * Mathf.Rad2Deg;

        float angle = 90 - angle2;
        //float angle = Vector3.Angle(dToR, targetToRoot);
        Vector3 axis = Vector3.Cross(dToR, targetToRoot);
        axis.Normalize();
        Quaternion newRotation = rootChildRotation * Quaternion.AngleAxis(angle, Quaternion.Inverse(rootChildRotation) * axis);
        Draw(newRotation, childLocalR, childLocalP, Color.black);
        DrawLine(transform.position, transform.position + axis, Color.magenta);
        Vector3 lastChildWoldP = transform.position + newRotation * childLocalP;
        DrawLine(lastChildWoldP, Target.position, Color.magenta);
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
