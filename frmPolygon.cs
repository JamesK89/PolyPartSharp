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

using PolyPartition;

namespace Polygon
{
    public sealed class frmPolygon : Form
    {
        private struct Triangle
        {
            public PointF Alpha, Bravo, Charlie;
        }

        private Drawable _drawable;
        private List<PointF> _points;

        private List<Triangle> _triangles;

        public frmPolygon()
            : base()
        {
            this.ClientSize = new Size(800, 600);
            this.Title = "Polygon";

            _drawable = new Drawable();
            _drawable.Paint += DrawablePaint;
            this.Content = _drawable;

            _triangles = new List<Triangle>();
            _points = new List<PointF>();
            this.MouseUp += FormMouseUp;

            this.Invalidate();
        }

        private void FormMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Buttons == MouseButtons.Alternate)
            {
                _triangles.Clear();
                _points.Clear();
            }
            else if (e.Buttons == MouseButtons.Middle)
            {
                Triangulate();
                _points.Clear();
            }
            else if (e.Buttons == MouseButtons.Primary)
            {
                _points.Add(e.Location);
            }

            this.Invalidate();
        }

        private void DrawablePaint(object sender, PaintEventArgs e)
        {
            using (SolidBrush blk = new SolidBrush(Color.FromArgb(0x00, 0x00, 0x00)))
            {
                e.Graphics.Clear(blk);

                using (Pen blue = new Pen(Color.FromArgb(0x00, 0xAA, 0xFF)), red = new Pen(Color.FromArgb(0xFF, 0x00, 0x00)), green = new Pen(Color.FromArgb(0x00, 0xFF, 0xAA)))
                {
                    if (_points.Count > 1)
                    {
                        for (int i = 0; i < _points.Count - (_points.Count < 3 ? 1 : 0); i++)
                        {
                            PointF current = _points[i];
                            PointF next = (i >= _points.Count - 1 ? _points[0] : _points[i + 1]);

                            e.Graphics.DrawLine(blue, current, next);
                        }

                        for (int i = 0; i < _points.Count; i++)
                        {
                            DrawPoint(e.Graphics, red, _points[i]);
                        }
                    }
                    else if (_triangles.Count > 0)
                    {
                        for (int i = 0; i < _triangles.Count; i++)
                        {
                            DrawTriangle(e.Graphics, green, red, _triangles[i]);
                        }
                    }
                }
            }
        }

        private void DrawPoint(Graphics graphics, Pen pen, PointF point)
        {
            graphics.DrawRectangle(pen, point.X - 1, point.Y - 1, 3, 3);
        }

        private void DrawTriangle(Graphics graphics, Pen linePen, Pen pointPen, Triangle triangle)
        {
            graphics.DrawLine(linePen, triangle.Alpha, triangle.Bravo);
            graphics.DrawLine(linePen, triangle.Bravo, triangle.Charlie);
            graphics.DrawLine(linePen, triangle.Charlie, triangle.Alpha);

            DrawPoint(graphics, pointPen, triangle.Alpha);
            DrawPoint(graphics, pointPen, triangle.Bravo);
            DrawPoint(graphics, pointPen, triangle.Charlie);
        }

        private void Triangulate()
        {
            TPPLPoly poly = new TPPLPoly();
            
            foreach (PointF p in _points)
            {
                poly.Points.Add(new TPPLPoint(p.X, p.Y));
            }

            poly.SetOrientation(TPPLOrder.CCW);

            List<TPPLPoly> triangles = new List<TPPLPoly>();
            TPPLPartition part = new TPPLPartition();
            part.Triangulate_EC(poly, triangles);

            _triangles.Clear();

            foreach(TPPLPoly triangle in triangles)
            {
                Triangle tr = new Triangle();

                triangle.SetOrientation(TPPLOrder.CW);

                tr.Alpha = new PointF(triangle[0].X, triangle[0].Y);
                tr.Bravo = new PointF(triangle[1].X, triangle[1].Y);
                tr.Charlie = new PointF(triangle[2].X, triangle[2].Y);

                _triangles.Add(tr);
            }
        }
    }
}
