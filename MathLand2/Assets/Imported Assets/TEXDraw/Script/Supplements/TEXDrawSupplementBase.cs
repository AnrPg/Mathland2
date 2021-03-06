using UnityEngine;
using System.Collections.Generic;
using System;

namespace TexDrawLib
{
    [ExecuteInEditMode]
    public abstract class TEXDrawSupplementBase : MonoBehaviour
    {

        MonoBehaviour target;
	    
	    /// Fast utility to get targeted TEXDraw component
        public ITEXDraw tex { get { return (ITEXDraw)target; } }

        protected virtual void OnEnable()
        {
            List<MonoBehaviour> l = ListPool<MonoBehaviour>.Get();
            GetComponents<MonoBehaviour>(l);
            for (int i = 0; i < l.Count; i++) {
                if (l[i] is ITEXDraw) {
                    target = l[i];
                    tex.SetSupplementDirty();
                    break;
                }
            }
            ListPool<MonoBehaviour>.ReleaseNoFlush(l);
        }

        protected virtual void OnDisable()
        {
            if (target)
                tex.SetSupplementDirty();
        }
        
                
        protected virtual void OnDestroy()
        {
            if (target)
                tex.SetSupplementDirty();
        }
	    
	    /// Inform to Apply changes and revalidate the graphic
	    public void ApplyChanges ()
	    {
		    if (target)
			    tex.SetTextDirty(true);
	    }

        #if UNITY_EDITOR
        void OnValidate()
        {
	        ApplyChanges();
        }
	    
	    /// Don't use this at runtime
        [ContextMenu("Copy to Clipboard")]
        public void Assert2Clipboard()
        {
            UnityEditor.EditorGUIUtility.systemCopyBuffer = ReplaceString(tex.text);
        }
        #endif

        public abstract string ReplaceString(string original);
    }
    
    [AttributeUsageAttribute(AttributeTargets.Class)]
    public class TEXSupHelpTip : Attribute {

        public string text;
        public bool beta;
        public TEXSupHelpTip (string Text, bool Beta = false) {
            text = Text;
            beta = Beta;
        }
    }
}