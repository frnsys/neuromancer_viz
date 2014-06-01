using UnityEngine;
using System.Collections;

// Thank you http://answers.unity3d.com/questions/45997/texture-animation.html
public class TextureAnimationScript : MonoBehaviour {

    public int TilesX;
    public int TilesY;
    public float AnimationCyclesPerSecond;

	// Use this for initialization
	void Start () {
        renderer.material.mainTextureScale = new Vector2(1.0f / TilesX, 1.0f / TilesY);
        StartCoroutine(ChangeOffSet());
	}

    IEnumerator ChangeOffSet ()
    {
       for(int i = TilesY - 1; i > -1; i--)
       {
         for(int j = 0; j < TilesX; j++)
         {
          renderer.material.mainTextureOffset = new Vector2(1.0f / TilesX * j, 1.0f / TilesY * i);
          yield return new WaitForSeconds(1.0f / (TilesX * TilesY * AnimationCyclesPerSecond));
          if(i == 0 && j == TilesX - 1)
          {
              i = TilesY;
          }
         }
       }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
