using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S1TRS : MonoBehaviour
{
    public Mesh m_mesh;
    public Material m_mat;
    public Vector3 m_position = Vector3.zero;
    public bool m_newMeshFlag = false;


    public bool m_moveFlag = false;
    public bool m_rotateFlag = false;
    public bool m_scaleFlag = false;

    [SerializeField] private float m_rotateSpeed = 1;
    [SerializeField] private Vector3 m_rotateN = Vector3.up;
    private float m_totalAngle = 0;

    [SerializeField] private float m_scaleK = 1;
    [SerializeField] private Vector3 m_scaleN = Vector3.zero;

    private Matrix4x4 m_transMat = Matrix4x4.identity;


    [SerializeField] private Vector3 m_testPoint = Vector3.one;


    private Matrix4x4 Rotate(Vector3 n, float r)
    {
        n.Normalize();
        float rad = Mathf.Deg2Rad * r;
        var mat = Matrix4x4.identity;
        Vector4 r1 = Vector4.zero;
        r1.x = n.x * n.x * (1 - Mathf.Cos(rad)) + Mathf.Cos(rad);
        r1.y = n.x * n.y * (1 - Mathf.Cos(rad)) + n.z * Mathf.Sin(rad);
        r1.z = n.x * n.z * (1 - Mathf.Cos(rad)) - n.y * Mathf.Sin(rad);
        r1.w = mat[0, 3];

        Vector4 r2 = Vector4.zero;
        r2.x = n.x * n.y * (1 - Mathf.Cos(rad)) - n.z * Mathf.Sin(rad);
        r2.y = n.y * n.y * (1 - Mathf.Cos(rad)) + Mathf.Cos(rad);
        r2.z = n.y * n.z * (1 - Mathf.Cos(rad)) + n.x * Mathf.Sin(rad);
        r2.w = mat[1, 3];

        Vector4 r3 = Vector4.zero;
        r3.x = n.x * n.z * (1 - Mathf.Cos(rad)) + n.y * Mathf.Sin(rad);
        r3.y = n.y * n.z * (1 - Mathf.Cos(rad)) - n.x * Mathf.Sin(rad);
        r3.z = n.z * n.z * (1 - Mathf.Cos(rad)) + Mathf.Cos(rad);
        r3.w = mat[2, 3];

        mat.SetColumn(0, r1);
        mat.SetColumn(1, r2);
        mat.SetColumn(2, r3);

        return mat;
    }

    private Matrix4x4 Sclae(Vector3 n, float k)
    {
        n.Normalize();
        var matr = Matrix4x4.identity;
        Vector4 r1 = matr.GetColumn(0);
        r1.x = 1 + (k - 1) * n.x * n.x;
        r1.y = (k - 1) * n.x * n.y;
        r1.z = (k - 1) * n.x * n.z;

        Vector4 r2 = matr.GetColumn(1);
        r2.x = (k - 1) * n.x * n.y;
        r2.y = 1 + (k - 1) * n.y * n.y;
        r2.z = (k - 1) * n.y * n.z;

        Vector4 r3 = matr.GetColumn(2);
        r3.x = (k - 1) * n.x * n.z;
        r3.y = (k - 1) * n.z * n.y;
        r3.z = 1 + (k - 1) * n.z * n.z;

        matr.SetColumn(0, r1);
        matr.SetColumn(1, r2);
        matr.SetColumn(2, r3);
        return matr;
    }



    private Matrix4x4 Move(Vector3 m)
    {
        var mat = Matrix4x4.identity;
        mat[0, 3] = m.x;
        mat[1, 3] = m.y;
        mat[2, 3] = m.z;
        return mat;
    }




    private void Start()
    {
        
    }

/*    private void onpostrender()
    {
        gl.clear(true, true, color.white);
        quaternion quat = quaternion.euler(m_angle);
        matrix4x4 m1 = matrix4x4.trs(m_position, quat, m_scale);
        if (m_newmeshflag)
        {
            var mesh = new mesh();
            mesh.vertices = new vector3[] { new vector3(0, 0, 0), new vector3(0, 1, 0), new vector3(1, 1, 0) };
            mesh.uv = new vector2[] { new vector2(0, 0), new vector2(0, 1), new vector2(1, 1) };
            mesh.triangles = new int[] { 0, 1, 2 };
            graphics.drawmesh(mesh, m1, m_mat, 0);
        }
        else
        {
            graphics.drawmesh(m_mesh, m1, m_mat, 0);
        }

    }*/

    private void Update()
    {
        m_totalAngle += Time.deltaTime * m_rotateSpeed;
        var ms = Sclae(m_scaleN, m_scaleK);
        var mr = Rotate(m_rotateN, m_totalAngle);
        var mm = Move(m_position);

        var mall = Matrix4x4.identity;

        if (m_rotateFlag)
        {
            mall *= mr;
        }
        if (m_scaleFlag)
        {
            mall *= ms;
        }
        if (m_moveFlag)
        {
            mall *= mm;
        }
        m_transMat = mall;


        if (m_newMeshFlag)
        {
            var mesh = new Mesh();
            mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0) };
            mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
            mesh.triangles = new int[] { 0, 1, 2 };
            Graphics.DrawMesh(mesh, m_transMat, m_mat, 0);
        }
        else
        {
            Graphics.DrawMesh(m_mesh, m_transMat, m_mat, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawMesh(m_mesh, m_position, m_transMat.rotation, Vector3.one);
        Gizmos.DrawLine(new Vector3(-10000, 0, 0), new Vector3(10000, 0, 0));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(0, -10000, 0), new Vector3(0, 10000, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(0, 0, -10000), new Vector3(0, 0, 10000));
        Gizmos.color = Color.white;
        Gizmos.DrawLine(-10 * m_rotateN, m_rotateN * 10);

        var point = m_transMat.MultiplyPoint(m_testPoint);
        Gizmos.DrawSphere(point, 0.1f);
    }
}

