using QRCoder;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace NcmlAtwServer {

    static class Utils {

        public static List<string> GetNetworkInterfaces() {
            var result = new List<string>();
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces()) {
                var containsIPv4 = false;
                foreach (var addr in ni.GetIPProperties().UnicastAddresses) {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork) {
                        containsIPv4 = true;
                        break;
                    }
                }
                if (containsIPv4) {
                    result.Add(ni.Description);
                }
            }
            return result;
        }

        public static string GetNetworkInterfaceIPv4(string description) {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces()) {
                if (ni.Description == description) {
                    foreach (var addr in ni.GetIPProperties().UnicastAddresses) {
                        if (addr.Address.AddressFamily == AddressFamily.InterNetwork) {
                            return addr.Address.ToString();
                        }
                    }
                }
            }
            return "unknown";
        }

        private const string QRCODE_MAGIC = "NCMLATW-";

        public static Bitmap GenerateAddressQrcode(string ip, int port, int pixelsPerModule = 10) {
            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode($"{QRCODE_MAGIC}{ip}:{port}", QRCodeGenerator.ECCLevel.Q);
            var code = new QRCode(data);
            return code.GetGraphic(pixelsPerModule);
        }

        private static Form _bitmapForm;

        public static void ShowBitmapForm(Bitmap bmp, string title, Form parent) {
            _bitmapForm?.Close();
            _bitmapForm = new Form {
                Text = title,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false,
                ShowIcon = false,
                StartPosition = FormStartPosition.CenterScreen,
                Size = new Size(bmp.Width, bmp.Height + (parent.RectangleToScreen(parent.ClientRectangle).Top - parent.Top - 8)),
            };

            var pictureBox = new PictureBox {
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = bmp,
                Dock = DockStyle.Fill,
            };
            _bitmapForm.Controls.Add(pictureBox);
            _bitmapForm.Show(parent);
        }

        public static void CloseBitmapForm() {
            _bitmapForm?.Close();
        }
    }
}
