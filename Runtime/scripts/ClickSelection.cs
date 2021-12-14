using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRSelection
{
    public class ClickSelection : SelectionModality
    {
        public ClickSelection(Selection selection) : base(selection) { }
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