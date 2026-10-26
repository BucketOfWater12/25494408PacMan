using UnityEngine;

public class StarBorder : MonoBehaviour
{

    [SerializeField] private RectTransform starPrefab;
    [SerializeField] private int starCount = 48;
    [SerializeField] private float speed = 150f;

    private RectTransform border;
    private RectTransform[] stars;
    private float offset;

    void Start()
    {
        border = GetComponent<RectTransform>();
        stars = new RectTransform[starCount];

        for (int i = 0; i < starCount; i++)
        {
            stars[i] = Instantiate(starPrefab, border);
        }
    }

    void Update()
    {
        Rect r = border.rect;
        float perimeter = 2f * (r.width + r.height);
        float spacing = perimeter / starCount;

        offset = Mathf.Repeat(offset + speed * Time.deltaTime, perimeter);

        for (int i = 0; i < starCount; i++)
        {
            float distance = Mathf.Repeat(offset + i * spacing, perimeter);
            stars[i].localPosition = PointOnPerimeter(r, distance);
        }
    }

    private Vector2 PointOnPerimeter(Rect r, float d)
    {
        if (d < r.width) return new Vector2(r.xMin + d, r.yMax);
        d -= r.width;
        
        if (d < r.height) return new Vector2(r.xMax, r.yMax - d);
        d -= r.height;

        if (d < r.width) return new Vector2(r.xMax - d, r.yMin);
        d -= r.width;

        return new Vector2(r.xMin, r.yMin + d);
    }
}
