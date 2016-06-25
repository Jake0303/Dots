using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class ColorExtensions : MonoBehaviour {

	public static Color ParseColor (string col){
         //Takes strings formatted with numbers and no spaces before or after the commas:
         // "1.0,1.0,.35,1.0"
         var strings = col.Split(","[0] );
         //Remove the characters 'RGBA' and brackets out of the string
         strings[0] = Regex.Replace(strings[0], "[A-Za-z()]", "");
         Color output = new Color();
         for (var i = 0; i < 4; i++) {
              output[i] = System.Single.Parse(strings[i].Replace(")", ""));
         }
         return output;
     }
 }

