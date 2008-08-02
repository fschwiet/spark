﻿/*
   Copyright 2008 Louis DeJardin - http://whereslou.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Spark.Parser.Markup;

namespace Spark.Compiler.NodeVisitors
{
    public class TestElseElementVisitor : AbstractNodeVisitor
    {

        private Frame _frame = new Frame { Nodes = new List<Node>() };
        readonly Stack<Frame> _stack = new Stack<Frame>();

        void PushFrame()
        {
            _stack.Push(_frame);
            _frame = new Frame();
        }
        void PopFrame()
        {
            _frame = _stack.Pop();
        }


        class Frame
        {
            public IList<Node> Nodes { get; set; }
            public IList<Node> TestParentNodes { get; set; }
        }

        public override IList<Node> Nodes
        {
            get { return _frame.Nodes; }
        }


        protected override void Visit(ExpressionNode node)
        {
            Nodes.Add(node);
        }

        protected override void Visit(EntityNode node)
        {
            Nodes.Add(node);
        }

        protected override void Visit(DoctypeNode node)
        {
            Nodes.Add(node);
        }

        protected override void Visit(TextNode node)
        {
            Nodes.Add(node);
        }

        protected override void Visit(ElementNode node)
        {
            Nodes.Add(node);
        }

        protected override void Visit(EndElementNode node)
        {
            Nodes.Add(node);
        }

        protected override void Visit(AttributeNode node)
        {
            Nodes.Add(node);
        }

        protected override void Visit(SpecialNode node)
        {
            bool detachFromParent = false;
            if (node.Element.Name == "else" &&
                node.Element.IsEmptyElement &&
                _frame.TestParentNodes != null)
            {
                detachFromParent = true;
            }

            if (detachFromParent)
            {
                var reconstructed = new SpecialNode(node.Element);
                _frame.TestParentNodes.Add(reconstructed);
                _frame.Nodes = reconstructed.Body;
            }
            else
            {
                var reconstructed = new SpecialNode(node.Element);
                Nodes.Add(reconstructed);

                var parentNodes = _frame.Nodes;
                
                PushFrame();

                _frame.Nodes = reconstructed.Body;
                if (node.Element.Name == "if" || node.Element.Name == "test")
                    _frame.TestParentNodes = parentNodes;

                Accept(node.Body);

                PopFrame();
            }
        }

        protected override void Visit(CommentNode node)
        {
            Nodes.Add(node);
        }

        protected override void Visit(ExtensionNode node)
        {
            Nodes.Add(node);
        }

        protected override void Visit(StatementNode node)
        {
            Nodes.Add(node);
        }

    }
}
