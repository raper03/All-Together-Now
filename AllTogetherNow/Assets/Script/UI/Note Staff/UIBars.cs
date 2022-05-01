using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Shapes;
[System.Serializable]
public struct LineLength
{
    public Vector2 start;
    public Vector2 end;
	public float thickness;
}
/// <summary>
/// Draws the lines to make bars of a staff.
/// </summary>
[ExecuteAlways] public class UIBars : ImmediateModeShapeDrawer {

    public Transform crosshairTransform;
    public LineLength[] linelengths;
	public LineLength[] divideLengths;
	public Vector2 starting_position;
	public int bars;
	public LineLength[] y_lines;
	public LineLength[] x_lines;
	public Camera renderCamera;
	public bool experimental;
	void Awake()
	{
		if(renderCamera == null)
			renderCamera = Camera.main;
		y_lines = new LineLength[bars + 1];
		float space_between = 1f / bars;
		float mag_number = starting_position.y - 0.27f;
		for(int i = 0; i < bars + 1; i++){
			if(i < 1){ 
				y_lines[i].start = starting_position;
				y_lines[i].end = new Vector2(starting_position.x, mag_number);
				continue;
			}
			
			y_lines[i].start = y_lines[i - 1].start;
			y_lines[i].end = y_lines[i - 1].end;

			y_lines[i].start.x += space_between;
			y_lines[i].end.x += space_between;
		}

		x_lines = new LineLength[10];
		float space_vertically = mag_number - starting_position.y;
		int dividend = x_lines.Length - 1;
		for(int i = 0; i < x_lines.Length; i++){
			
			if(i < 1){
				x_lines[i].start = starting_position;
				x_lines[i].end = new Vector2(starting_position.x + 1, starting_position.y);
				continue;
			}

			x_lines[i].start = x_lines[i - 1].start ;
			x_lines[i].end = x_lines[i - 1].end;

			x_lines[i].start.y += space_vertically / (float)dividend;
			x_lines[i].end.y += space_vertically / (float)dividend;


		}

	}
	public override void DrawShapes( Camera cam ){

		using( Draw.Command( cam ) ){

            Draw.ZTest = CompareFunction.Always; 
            Draw.Matrix = crosshairTransform.localToWorldMatrix;
			// set up static parameters. these are used for all following Draw.Line calls
			Draw.LineGeometry = LineGeometry.Flat2D;
			Draw.ThicknessSpace = ThicknessSpace.Pixels;
			Draw.Thickness = 4; // 4px wide
			
			if(experimental){

				
				y_lines = new LineLength[bars + 1];
				float space_between = 1f / bars;
				float mag_number = starting_position.y - 0.27f;
				for(int i = 0; i < bars + 1; i++){
					if(i < 1){ 
						y_lines[i].start = starting_position;
						y_lines[i].end = new Vector2(starting_position.x, mag_number);
						continue;
					}
					
					y_lines[i].start = y_lines[i - 1].start;
					y_lines[i].end = y_lines[i - 1].end;

					y_lines[i].start.x += space_between;
					y_lines[i].end.x += space_between;
				}

				x_lines = new LineLength[10];
				float space_vertically = mag_number - starting_position.y;
				int dividend = x_lines.Length - 1;
				for(int i = 0; i < x_lines.Length; i++){
					
					if(i < 1){
						x_lines[i].start = starting_position;
						x_lines[i].end = new Vector2(starting_position.x + 1, starting_position.y);
						continue;
					}

					x_lines[i].start = x_lines[i - 1].start ;
					x_lines[i].end = x_lines[i - 1].end;

					x_lines[i].start.y += space_vertically / (float)dividend;
					x_lines[i].end.y += space_vertically / (float)dividend;


				}

				foreach(LineLength _line in y_lines)
				{
					Draw.Line(_line.start, _line.end, Color.white);
				}
				foreach(LineLength _line in x_lines)
				{
					Draw.Line(_line.start, _line.end, Color.white);
				}

			}


		}

	}

}