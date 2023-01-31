using System.Drawing.Imaging;
using System.Drawing.Printing;
using BarcodeLib;

namespace PrintBarcode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class OpcionCombo
        {
            public int Valor { get; set; }
            public string Texto { get; set; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int indice = 0;
            foreach(var nombre in Enum.GetNames(typeof(BarcodeLib.TYPE)))
            {
                cbotipo.Items.Add(new OpcionCombo() { Valor = indice, Texto = nombre});
                indice++;
            }

            cbotipo.DisplayMember = "Texto";
            cbotipo.ValueMember = "Valor";
            cbotipo.SelectedIndex = 31;
        }

        private void btnBarcode_Click(object sender, EventArgs e)
        {
            Image imagencodigo;
            int indice = (cbotipo.SelectedItem as OpcionCombo).Valor;
            BarcodeLib.TYPE tipocodigo = (BarcodeLib.TYPE)indice;
            Barcode codigo = new Barcode();
            codigo.IncludeLabel = true;
            codigo.LabelPosition = LabelPositions.BOTTOMCENTER;
            imagencodigo = codigo.Encode(tipocodigo, txtBarcode.Text, Color.Black, Color.White, 300, 100);

            Bitmap imagenTitulo = convertirTextoImagen(txttitulo.Text, 300, Color.White);
            int alto_imagen_nuevo = imagencodigo.Height + imagenTitulo.Height;
            Bitmap imagenNueva = new Bitmap(300, alto_imagen_nuevo);
            Graphics dibujar = Graphics.FromImage(imagenNueva);
            dibujar.DrawImage(imagenTitulo, new Point(0, 0));
            dibujar.DrawImage(imagencodigo, new Point(0, imagenTitulo.Height));




            pictureBox1.BackgroundImage = imagenNueva;


        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            PrintDocument pDoc = new PrintDocument();
            pDoc.PrintPage += PrintPicture;
            pd.Document = pDoc; 
            if (pd.ShowDialog() == DialogResult.OK)
            {
                pDoc.Print();

            }
        }

        private void PrintPicture(object sender, PrintPageEventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.DrawToBitmap(bmp, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            e.Graphics.DrawImage(bmp, 0, 0);
            bmp.Dispose();
        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            Image imagen_codigo = pictureBox1.BackgroundImage.Clone() as Image;

            SaveFileDialog ventana_dialogo = new SaveFileDialog();
            ventana_dialogo.FileName = string.Format("{0}.png", txtBarcode.Text);
            ventana_dialogo.Filter = "Image |*.png";

            if(ventana_dialogo.ShowDialog() == DialogResult.OK)
            {
                imagen_codigo.Save(ventana_dialogo.FileName, ImageFormat.Png);
                MessageBox.Show("Imagen Guardada");
            }
        }

        public static Bitmap convertirTextoImagen(string texto, int ancho, Color color)
        {
            //creamos el objeto imagen Bitmap
            Bitmap objBitmap = new Bitmap(1, 1);
            int Width = 0;
            int Height = 0;
            //formateamos la fuente (tipo de letra, tamaño)
            System.Drawing.Font objFont = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);

            //creamos un objeto Graphics a partir del Bitmap
            Graphics objGraphics = Graphics.FromImage(objBitmap);

            //establecemos el tamaño según la longitud del texto
            Width = ancho;
            Height = (int)objGraphics.MeasureString(texto, objFont).Height + 5;
            objBitmap = new Bitmap(objBitmap, new Size(Width, Height));

            objGraphics = Graphics.FromImage(objBitmap);

            objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            objGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            objGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            StringFormat drawFormat = new StringFormat();
            objGraphics.Clear(color);

            drawFormat.Alignment = StringAlignment.Center;
            objGraphics.DrawString(texto, objFont, new SolidBrush(Color.Black), new RectangleF(0, (objBitmap.Height / 2) - (objBitmap.Height - 10), objBitmap.Width, objBitmap.Height), drawFormat);
            objGraphics.Flush();


            return objBitmap;
        }
    }

}