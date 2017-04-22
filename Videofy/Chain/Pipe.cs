﻿using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Videofy.Chain
{
    class Pipe
    {
        private BlockingCollection<byte[]> data;
        private Queue<byte> queue;
        //private byte[] buff = null;
        private int pos;
        private CancellationToken token;

        public Pipe(CancellationToken token)
        {
            this.token = token;
            data = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>(), 5);
            queue = new Queue<byte>();
            pos = 0;
            //buff = new byte[0];                        
        }

        public int Count { get { return queue.Count; } private set { } }

        public void Complete()
        {
            data.CompleteAdding();
        }

        public Boolean IsOpen { get { return !data.IsCompleted; } }

        //take data of any available size
        public byte[] Take()
        {
            byte[] temp = null;

            if (queue.Count > 0)
            {
                temp = queue.ToArray();
                queue.Clear();
            }
            else
            {
                try
                {
                    temp = data.Take(token);
                }
                catch (InvalidOperationException e)
                {
                    if (!((token.IsCancellationRequested)|(data.IsCompleted)))
                    {
                        throw e;
                    }
                }
            }
            return temp;
        }

        // take only data of expected size
        public byte[] Take(int size)
        {
            byte[] temp = null; 

            while (queue.Count < size)
            {
                try
                {
                    temp = data.Take(token);
                }
                catch (InvalidOperationException e)
                {
                    if (!((token.IsCancellationRequested) | (data.IsCompleted)) )
                    {
                        throw e;
                    }
                    // pipe is finalized, append zeros to tail 
                    

                    int cnt = queue.Count;
                    //Array.Resize<byte>(ref buff, size);
                    for(int i = cnt; i < size; i++)
                    {
                        queue.Enqueue(0);
                    }
                    temp = null;           
                }

                if(temp!=null)
                {
                    for(int i = 0; i < temp.Length; i++)
                    {
                        queue.Enqueue(temp[i]);
                    }                    
                }
            }

            if (queue.Count>= size)
            {
                temp = new byte[size];
                for(int i=0;i<size;i++)
                {
                    temp[i] = queue.Dequeue();  
                }
                
                return temp;
            }
            else
            {
                throw new Exception("THIS LINE MUST NOT BE EXECUTED");
            }
            
        }



        public void Add(byte[] value)
        {
            if (value == null) throw new ArgumentNullException();
            if (value.Length == 0) throw new ArgumentException();
            data.Add(value,token);
        }
    }
}
