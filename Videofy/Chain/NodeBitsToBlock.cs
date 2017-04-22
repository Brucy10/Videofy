﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Main;

namespace Videofy.Chain
{
    class NodeBitsToBlock:NodeBlockBase
    {

        public NodeBitsToBlock(OptionsStruct opt,Pipe input,Pipe output):base(opt,input,output)
        {

        }

        public override void Start()
        {
            StartBitsToBlock();
        }
    }
}
