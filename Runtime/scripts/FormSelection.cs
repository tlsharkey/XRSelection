using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRSelection
{
    public class FormSelection : SelectionModality
    {
        public FormSelection(Selection selection) : base(selection) { }
        public override void Start()
        {
            throw new System.NotImplementedException();
        }

        public override Transform[] Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}