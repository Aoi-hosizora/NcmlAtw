using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NeteaseM2DServer.Src.Util
{
    class CommonUtil
    {

        #region 窗口穿透

        private const uint WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;

        private const int GWL_EXSTYLE = (-20);
        private const int LWA_ALPHA = 0x2;

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        public static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        public static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        // bAlpha: 0 .. 255
        public static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

        /// <summary>
        /// 设置窗口穿透
        /// </summary>
        /// <param name="opacity">this.Opacity</param>
        /// <param name="isCross">是否穿透</param>
        public static void setWindowCrossOver(Form form, double opacity, bool isCross) {
            uint intExTemp = CommonUtil.GetWindowLong(form.Handle, GWL_EXSTYLE);
            uint oldGWLEx;
            if (isCross)
                oldGWLEx = CommonUtil.SetWindowLong(form.Handle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED);
            else
                oldGWLEx = CommonUtil.SetWindowLong(form.Handle, GWL_EXSTYLE, WS_EX_LAYERED);

            CommonUtil.SetLayeredWindowAttributes(form.Handle, 0, (int)(opacity * 255), LWA_ALPHA);
        }

        #endregion // 窗口穿透

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns>13位时间戳</returns>
        public static long GetTimeStamp() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Int64.Parse(Convert.ToInt64(ts.TotalMilliseconds).ToString());
        }

        /// <summary>
        /// 光标在控件范围内
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public static bool ControlInRange(Control parent, Control ctrl) {
            return Cursor.Position.X > parent.Left + ctrl.Left &&
                Cursor.Position.X < parent.Left + ctrl.Left + ctrl.Width &&
                Cursor.Position.Y > parent.Top + ctrl.Top &&
                Cursor.Position.Y < parent.Top + ctrl.Top + ctrl.Height;
        }
    }
}
