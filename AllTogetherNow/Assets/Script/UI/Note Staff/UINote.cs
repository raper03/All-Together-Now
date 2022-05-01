using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using Shapes;
public class Lengths
{
    public Vector3 start;
    public Vector3 end;
      
    public Lengths(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }
}
/// <summary>
/// Creates note nodes that resemble white bars to draw on the staff.
/// </summary>
[ExecuteAlways] public class UINote : ImmediateModeShapeDrawer
{
    
    public float noteVerticalPosition;
    public List<Lengths> drawn_notes;
    private List<Lengths>[] note_nodes;
    public Transform crosshairTransform;


    public void Awake()
    {
        drawn_notes = new List<Lengths>();
        note_nodes = new List<Lengths>[8 * 100];
        for(int i = 0; i < note_nodes.Length; i++)
        {
            note_nodes[i] = new List<Lengths>();
        }
        noteVerticalPosition = GetComponent<BoxCollider>().center.y;
        if(crosshairTransform == null)
            crosshairTransform = transform.parent;   
    }

    public void CreateNoteNode(float line_start, float line_end, int start_index, int length, bool cache)
    {
        if((start_index + length) >= note_nodes.Length) return;
        else if(start_index < 0) return;
        Debug.Log("Note Node Created!");
        foreach(Lengths len in drawn_notes)
        {
            
            Debug.Log(len);
        }
        Lengths l = new Lengths(new Vector3(line_start, noteVerticalPosition, 0), new Vector3(line_end, noteVerticalPosition, 0));
        drawn_notes.Add(l);

        if(!cache) return;    
        Debug.Log("caching " + (start_index + length));
        for(int i = start_index; i <= (start_index + length); i++){
            note_nodes[i].Add(l);
        }

        Debug.Log(drawn_notes[drawn_notes.Count - 1].Equals(note_nodes[start_index][note_nodes[start_index].Count - 1]));
    }

    public Lengths PopNoteNode(int start_index)
    {
        if(note_nodes.Length <= start_index || start_index < 0) return null;
        Lengths l = note_nodes[start_index][note_nodes[start_index].Count - 1];
        string message = drawn_notes.Contains(l) ? "node exists." : "node does not exist";
        Debug.Log(message);
        drawn_notes.Remove(l);

        Lengths cleared_node = l;
        Lengths new_node = new Lengths(cleared_node.start, cleared_node.end);

        
        for(int i = 0; i < note_nodes.Length; i++)
        {   
            note_nodes[i].Remove(cleared_node);
        }

        return new_node;
    }

    public void Clear()
    {
        drawn_notes.Clear();

        for(int i = 0; i < note_nodes.Length; i++)
        {
            note_nodes[i].Clear();
        }
    }
	public override void DrawShapes( Camera cam ){

		using( Draw.Command( cam ) ){
            Draw.ZTest = CompareFunction.Always; 
            Draw.Matrix = crosshairTransform.localToWorldMatrix;
            Draw.BlendMode = ShapesBlendMode.Transparent;
            
            Draw.LineGeometry = LineGeometry.Flat2D;
            Draw.Thickness = 0.03f;
            Draw.LineEndCaps = LineEndCap.None;
            Draw.Radius = 0.03f;
            if(!Application.isPlaying) return;
            
            float alpha = 0.75f;                

            for(int i = 0; i < drawn_notes.Count; i++){
                
                if(drawn_notes[i] == null) return;
                
                drawn_notes[i].start.y = noteVerticalPosition;
                drawn_notes[i].end.y =  noteVerticalPosition;

                Draw.Line(drawn_notes[i].start, drawn_notes[i].end, new Color(1,1,1,alpha));
            }
        }

	}
    
}
