﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public partial class TaskThumbnailView : UserControl
    {
        public List<TaskThumbnailPanel> Panels { get; private set; }
        public TaskThumbnailPanel SelectedPanel { get; private set; }
        public Size ThumbnailSize { get; set; } = new Size(200, 150);

        public delegate void TaskViewMouseEventHandler(object sender, MouseEventArgs e, WorkerTask task);
        public event TaskViewMouseEventHandler ContextMenuRequested;

        public TaskThumbnailView()
        {
            Panels = new List<TaskThumbnailPanel>();

            InitializeComponent();
            UpdateTheme();
        }

        protected override Point ScrollToControl(Control activeControl)
        {
            return AutoScrollPosition;
        }

        public void UpdateTheme()
        {
            if (ShareXResources.UseDarkTheme)
            {
                BackColor = ShareXResources.DarkBackgroundColor;
            }
            else
            {
                BackColor = SystemColors.Window;
            }

            foreach (TaskThumbnailPanel panel in Panels)
            {
                panel.UpdateTheme();
            }
        }

        public TaskThumbnailPanel FindPanel(WorkerTask task)
        {
            return Panels.FirstOrDefault(x => x.Task == task);
        }

        public void AddTaskPanel(WorkerTask task)
        {
            TaskThumbnailPanel panel = new TaskThumbnailPanel(task);
            panel.ChangeThumbnailSize(ThumbnailSize);
            panel.MouseDown += (sender, e) => SelectedPanel = panel;
            panel.MouseUp += Panel_MouseUp;
            Panels.Add(panel);
            flpMain.Controls.Add(panel);
            flpMain.Controls.SetChildIndex(panel, 0);
        }

        public void RemoveTaskPanel(WorkerTask task)
        {
            TaskThumbnailPanel panel = FindPanel(task);

            if (panel != null)
            {
                Panels.Remove(panel);
                flpMain.Controls.Remove(panel);
                panel.Dispose();
            }
        }

        protected void OnContextMenuRequested(object sender, MouseEventArgs e, WorkerTask task)
        {
            if (ContextMenuRequested != null)
            {
                ContextMenuRequested(sender, e, task);
            }
        }

        private void FlpMain_MouseDown(object sender, MouseEventArgs e)
        {
            SelectedPanel = null;
        }

        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                OnContextMenuRequested(sender, e, SelectedPanel?.Task);
            }
        }
    }
}