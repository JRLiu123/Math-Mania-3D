using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyNamespace
{

    class PriorityQueue
    {
        private Node head;

        public PriorityQueue()
        {
            head = null;
        }

        public void Enqueue(Node newNode)
        {
            if (head == null || head.fScore > newNode.fScore)
            {
                newNode.Next = head;
                head = newNode;
            }
            else
            {
                Node current = head;
                while (current.Next != null && current.Next.fScore < newNode.fScore)
                {
                    current = current.Next;
                }

                newNode.Next = current.Next;
                current.Next = newNode;
            }
        }

        public void Dequeue()
        {
            if (head == null)
            {
                throw new InvalidOperationException("Queue is empty");
            }
            head = head.Next;
        }

        public Node Peek()
        {
            if (head == null)
            {
                throw new InvalidOperationException("Queue is empty");
            }
            return head;
        }
        public bool IsEmpty()
        {
            return head == null;
        }

        public int Count()
        {
            int i = 0;
            Node curNode = head;
            while (curNode != null)
            {
                i++;
                curNode = curNode.Next;
            }
            return i;
        }

        public float small_h()
        {
            float h = 1000;
            Node curNode = head;
            while (curNode != null)
            {
                if (curNode.hScore < h)
                {
                    h = curNode.hScore;
                }
                curNode = curNode.Next;
            }
            return h;

        }

        public bool cell_in_queue(int w, int l)  // check whether the incoming cell is already in queue
        {
            Node curNode = head;
            while(curNode != null)
            {
                if (curNode.cell[0] == w && curNode.cell[1] == l)
                {
                    return true;
                }
                curNode = curNode.Next;
            }
            return false;
        }

    }


}

