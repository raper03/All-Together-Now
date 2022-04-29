using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
enum Drag
{
    Whole,
    Right,
    Left
}

public class StaffEditor : MonoBehaviour
{
    
    public UnityAction<Player, int, float, float> rowEdit;
    public Action FinishInit;
    //hardcoded, depends on where the first horizontal line is on the staff, it just happens to be
    //-0.6f right now when viewing it through the camera.
    private float startingPosition = -.6f;
    private UINote[] graphicalNotes;
    public Vector4 frame_size;
    public Rect actual_frame_size;
    public Dictionary<UINote, int[]> row_resolution;

    private Player _currentPlayer;
    private bool refresh = true;
    private Drag drag = Drag.Whole;
    public void Start()
    {
        UIBars grid = GetComponent<UIBars>();
        frame_size = new Vector4(grid.x_lines[0].end.x, grid.y_lines[0].start.y, grid.y_lines[0].end.x, grid.y_lines[0].end.y);
        
        actual_frame_size = new Rect (
        x : grid.x_lines[0].start.x, 
        y : grid.y_lines[0].end.x, 
        width: (grid.x_lines[0].end - grid.x_lines[0].start).magnitude, 
        height: (grid.y_lines[0].end - grid.y_lines[0].start).magnitude );
        
        drag = Drag.Whole;
        graphicalNotes = gameObject.GetComponentsInChildren<UINote>();
        
        row_resolution = new Dictionary<UINote, int[]>();
        for(int i = 0; i < graphicalNotes.Length; i++){
            int[] occurences = new int[8 * 100];
            for(int j = 0; j < occurences.Length; j++)
                occurences[j] = -1;


            row_resolution.Add(graphicalNotes[i], occurences);

        }

        FinishInit?.Invoke();
    }

    public void Engage(){

    }
    public void Join(Player player)
    {
        _currentPlayer = player;
        
        ClearRow();
        StopCoroutine("FillRows");
        InterpretRow(player.notation.RequestNotes(), Color.white);
        
    }

    public void InterpretRow(List<Note>[] rows, Color color)
    {    
        StartCoroutine(FillRows(rows));
    }

    IEnumerator FillRows(List<Note>[] rows)
    {

        for(int n = 0; n < rows.Length; n++)
        {
            for(int j = 0; j < rows[n].Count; j++){
                UINote toEdit = graphicalNotes[rows[n][j].noteIndex];

                float x = rows[n][j].noteStart;
                float y = toEdit.noteVerticalPosition;
                float offset = x + rows[n][j].noteDuration;

                // -.6 + (0.0625*2)
                // 0.0625 = 1/16
                // 2 '1/16ths' = 1 seconds

                Vector3 graphicalNoteStart = new Vector3(startingPosition + (float)(x * (0.0625 * 2)), y, 0);
                Vector3 graphicalNoteEnd =  new Vector3(startingPosition + (float)(offset * (0.0625 * 2 )), y, 0);
                Lengths note = new Lengths(graphicalNoteStart, graphicalNoteEnd);
                
                int start = (int)(rows[n][j].noteStart * 100);
                int duration = (int)((rows[n][j].noteStart + rows[n][j].noteDuration) * 100);
                toEdit.CreateNoteNode(note.start.x, note.end.x, start, duration - start, true);            
                
                
                for(int i = start; i < duration; i++){
                    
                    row_resolution[toEdit][i] += 1;

                }

                yield return null;
            }
            yield return null;
        }
        yield return null;
    }
    public void ClearRow()
    {
        Debug.Log("Clearing row");

        foreach(UINote row in graphicalNotes)
        {
            row.Clear();
        }

        foreach(UINote note in graphicalNotes){
            row_resolution.Remove(note);
            int[] occurences = new int[8 * 100];
            for(int i = 0; i < occurences.Length; i++)
                occurences[i] = -1;
            row_resolution.Add(note, occurences);
        }


        
    }

    public void IndexNote(Vector3 pos, UINote row)
    {
        float convert = (pos.x - startingPosition) / (0.0625f * 2);
        int index = (int)(convert * 100);
        
        Lengths memory_line = row.drawn_notes[row.drawn_notes.Count - 1];
        Lengths user_line = new Lengths(memory_line.start, memory_line.end);
        row.PopNoteNode(previous_index);
        Debug.Log("out of popping");
        
        
        for(int i = 0; i < graphicalNotes.Length; i++)
        {
            if(row == graphicalNotes[i]){
                float original_time = (user_line.start.x - startingPosition) / (0.0625f * 2);
                Debug.Log(original_time);
                rowEdit.Invoke(_currentPlayer, i, original_time, 1);    
            }
        }

        float nls = (user_line.start.x - startingPosition) / (0.0625f * 2);  
        Debug.Log("nls: " +nls);
        float nle = (user_line.end.x - startingPosition) / (0.0625f * 2);
        float snflsndfl = (nls * 100);
        float uiaugiafu = (nle * 100);
        int in_s = (int)snflsndfl;
        int ie = (int)uiaugiafu;

        Debug.Log("in_s " + in_s + " ie" + ie + " " + (ie - in_s));
        
        row.CreateNoteNode(user_line.start.x, user_line.end.x, in_s, ie - in_s, true);
        for(int i = in_s; i <= ie ; i++)
        {
            row_resolution[row][i] += 1;
        }

        row.drawn_notes.Remove(user_line);
        
        refresh = true;
        drag = Drag.Whole;

        currentRow = null;
        index = -1;  
    }

    #nullable enable
    private UINote? currentRow = null;
    private Vector3 currentPos;
    #nullable disable

    private int previous_index;
    public float tolerance;
    private float oldDistance = 0.0925f;
    public void MoveNoteToPosition(Vector3 pos, UINote row)
    {
        pos.x = Mathf.Clamp(pos.x, startingPosition, frame_size.x);
        currentRow = currentRow ?? row;
        if(currentRow != row && (drag != Drag.Right && drag != Drag.Left)){
            Debug.Log("Previous row: " + currentRow + "Row is now: " + row);
            
            
            Lengths l = currentRow.PopNoteNode(previous_index);
            
            currentRow.drawn_notes.Remove(l);
            currentRow = row;
            
            float ls = (l.start.x - startingPosition) / (0.0625f * 2);
            float le = (l.end.x   - startingPosition) / (0.0625f * 2);
            float nls = ls * 100;
            float nle = le * 100;
            int index_s = (int)nls;
            int index_e = (int)nle;
            
            currentRow.CreateNoteNode(l.start.x, l.end.x, index_s, index_e - index_s, true);
            
            refresh = true;
            drag = Drag.Whole;

            previous_index = index_s;
        }
        if(refresh)
        {
            currentPos = pos;
            refresh = false;
        }
        else{
            Lengths currentLength = currentRow.drawn_notes[currentRow.drawn_notes.Count - 1];
            Vector3 left = currentLength.start;
            Vector3 right = currentLength.end;
            
            oldDistance = Vector3.Distance(left, right);

            float oldPosDistance = pos.x - currentPos.x;
            
            //float direction = (pos - currentPos).normalized.x < 0 ? -1 : 1;
            //Debug.Log("left: " + Vector3.Distance(left, pos) + " right: " + Vector3.Distance(right, pos));
            float distanceLeft = oldDistance - Vector3.Distance(left, pos);
            float distanceRight = oldDistance - Vector3.Distance(right, pos);
            
            //Debug.LogFormat("{0}, {1}, {2}", new object[3]{oldPosDistance, distanceLeft, distanceRight});
            
            if(drag == Drag.Whole)
            {
                distanceLeft = oldPosDistance;
                distanceRight = oldPosDistance;
            }
            else if(distanceLeft <= tolerance || drag == Drag.Right)
            {
                distanceRight = oldPosDistance;
                distanceLeft = 0;
                drag = Drag.Right;
            }
            else if(distanceRight <= tolerance || drag == Drag.Left)
            {
                
                distanceLeft = oldPosDistance;
                distanceRight = 0;
                drag = Drag.Left;
            }

            
            left.x += distanceLeft;
            right.x += distanceRight;

            currentLength.start = left;
            currentLength.end = right;
            currentPos = pos;

            
            
        }

    }

    public void CheckExistingNote(Vector3 pos, UINote row)
    {
        Debug.Log("indexing");
        //turn pos into index
        //Create Note Node(start, length, index)
        //cache Node
        //in MoveNoteToPosition() set new index based on where we move it, update position graphically


        float convert = (pos.x - startingPosition) / (0.0625f * 2);
        int index = (int)(convert * 100);
        
        if(index > 8 * 100 || index < 0) return;
        else if(row_resolution[row][index] < 0){
            //create note
            float nls = (pos.x - startingPosition) / (0.0625f * 2);            
            float nle = ((pos.x + oldDistance)  - startingPosition) / (0.0625f * 2);
            float snflsndfl = (nls * 100);
            float uiaugiafu = (nle * 100);
            int in_s = (int)snflsndfl;
            int ie = (int)uiaugiafu;

            row.CreateNoteNode(pos.x, pos.x + oldDistance, index, ie - in_s, true);
            
            previous_index = index;
            currentRow = row;
            Debug.Log("previous Index is now: " + previous_index);
            return;
        }
        else if(row_resolution[row].Length < 1) return;
        
        Debug.Log("Note already exists, using that one.");
        Lengths l = row.PopNoteNode(index);
        float ls = (l.start.x - startingPosition) / (0.0625f * 2);
        float le = (l.end.x   - startingPosition) / (0.0625f * 2);

        int index_s = (int)(ls * 100);
        Debug.Log("index_s " + index_s);
        int index_e = (int)(le * 100);
        row.CreateNoteNode(l.start.x, l.end.x, index, index_e - index_s, false);



    }

    public void RemoveNote(Vector3 pos, UINote row)
    {
        
        float convert = (pos.x - startingPosition) / (0.0625f * 2);
        int index = (int)(convert * 100);
        Debug.Log("Proceeding to delete " + index + " " + row_resolution[row][index]);
        if(index > 8 * 100 || index < 0) return;
        else if(row_resolution[row][index] < 0) return;
        else if(row_resolution[row].Length < 1) return;
        
        Debug.Log("Deletion at index: " + index);

        Lengths line = row.PopNoteNode(index);
        
        float line_start_convert = (line.start.x - startingPosition) / (0.0625f * 2);
        float line_end_convert = (line.end.x - startingPosition) / (0.0625f * 2);

        int start_index = (int)(line_start_convert * 100);
        int end_index = (int)(line_end_convert * 100);

        for(int h = start_index; h < end_index; h++)
            row_resolution[row][h] -= 1;
        
        for(int i = 0; i < graphicalNotes.Length; i++)
        {
            if(row == graphicalNotes[i]){
                float original_time = (line.start.x - startingPosition) / (0.0625f * 2);
                rowEdit.Invoke(_currentPlayer, i, original_time, -1f);    
            }
        }
    }
}
