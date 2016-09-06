// ============================================================================================================
// PolyPartSharp: library for polygon partition and triangulation based on the PolyPartition C++ library 
// https://github.com/JamesK89/PolyPartSharp
// Original project: https://github.com/ivanfratric/polypartition
// ============================================================================================================
// Original work Copyright (C) 2011 by Ivan Fratric
// Derivative work Copyright (C) 2016 by James John Kelly Jr.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Eto;
using Eto.Forms;
using Eto.Drawing;

namespace Polygon
{
    public sealed class Application
    {
        [STAThread]
        private static int Main(string[] args)
        {
            Instance = new Application();
            return Instance.Return;
        }

        public static Application Instance
        {
            get;
            private set;
        }

        public int Return
        {
            get;
            private set;
        }

        public List<Form> Forms
        {
            get;
            private set;
        }

        private Application()
        {
            try
            {
                Application.Instance = this;
                
                (new Eto.Forms.Application(Platform.Detect)).Attach();

                Forms = new List<Form>();

                CreateForm<frmPolygon>().Show();
                
                do
                {
                    System.Threading.Thread.Sleep(1);
                    Eto.Forms.Application.Instance.RunIteration();
                } while (Forms.Count > 0);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                Eto.Forms.MessageBox.Show(e.ToString(), "Unhandled Exception", MessageBoxType.Error);
                Return = 1;
            }
        }

        public Form CreateForm<T>() where T: Form
        {
            T nForm = Activator.CreateInstance<T>();

            nForm.Closed += (object s, EventArgs e) =>
            {
                Application.Instance.Forms.Remove((Form)s);
            };

            Application.Instance.Forms.Add(nForm);

            return (Form)nForm;
        }
    }
}
