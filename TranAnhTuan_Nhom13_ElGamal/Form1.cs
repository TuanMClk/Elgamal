using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
namespace TranAnhTuan_Nhom13_ElGamal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        
        public BigInteger p, a, d, x, k, y, y2;
        OpenFileDialog openFileDialog = new OpenFileDialog();
        //OpenFileDialog open;
      /*  private void btnOpenFileKtraChuKy_Click(object sender, EventArgs e)
        {
            openFileDialog.InitialDirectory = "All files (*.*)|*.*|All files(*.*) | *.* ";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFileKiemTraChuKySo.Text = openFileDialog.FileName;
                string pathSignature = Path.ChangeExtension(openFileDialog.FileName, "sig");
                if (File.Exists(pathSignature))
                {
                    openFileDialog.FileName = pathSignature;
                    txtFileChuKy.Text = pathSignature;
                }
            }    
        }*/

        private void btnOpenFileThucHienKy_Click(object sender, EventArgs e)
        {
             openFileDialog.InitialDirectory = "All files (*.*)|*.*|All files(*.*) | *.* ";
            // if (openFileDialog.ShowDialog() == DialogResult.OK)
            //openFileDialog.Filter = "|*.txt";
            richTextBox1.Clear();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                object filename = @openFileDialog.FileName;
                Microsoft.Office.Interop.Word.Application AC = new Microsoft.Office.Interop.Word.Application();
                Microsoft.Office.Interop.Word.Document doc = new Microsoft.Office.Interop.Word.Document();
                object readOnly = false;
                object isVisible = true;
                object missing = System.Reflection.Missing.Value;
                try
                {
                    doc = AC.Documents.Open(ref filename, ref missing, ref readOnly, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref isVisible);
                    doc.Content.Select();
                    doc.Content.Copy();
                    richTextBox1.Paste();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message);
                }
                finally
                {
                    doc.Close(ref missing, ref missing, ref missing);
                }
                StreamReader read = new StreamReader(openFileDialog.FileName);
                 txtFileThucHienKy.Text  = read.ReadToEnd();
                read.Close();
                string fileName = openFileDialog.FileName;
                txtFile.Text = fileName;
                using (Stream stream = new FileStream(fileName, FileMode.Open))
                {
                    SHA256Managed sha256Managed = new SHA256Managed();
                    byte[] hashBytes = sha256Managed.ComputeHash(stream);
                    string digest = Convert.ToBase64String(hashBytes);
                    txtChuKyBaoVanBan.Text = digest.ToString();
                }
            }    
        }

        //Tinh A^k mod n
        static public BigInteger power(BigInteger a, BigInteger exp, BigInteger mod)
        {
            if (exp < 0) return 0;
            else if (exp == 0) return 1;
            var r = power(a, exp >> 1, mod);
            var ans = (r * r) % mod;
            if ((exp & 1) == 1) ans = (ans * a) % mod;
            return ans;
        }

        static private BigInteger[] heSo(BigInteger n)
        {
            long s = 0;
            while ((n & 1) == 0)
            {
                s++;
                n >>= 1;
            }
            return new BigInteger[] { s, n };
        }

        static private bool checkMillerRabin(BigInteger s, BigInteger d, BigInteger n, BigInteger a)
        {
            if (n == a) return true;
            var p = power(a, d, n);
            if (p == 1) return true;
            while (s > 0)
            {
                if (p == n - 1) return true;
                p = p * p % n;
                s--;
            }
            return false;
        }

        private static bool KiemTraSoNguyenTo(BigInteger n)
        {
            if (n < 2) return false;
            if ((n & 1) == 0) return n == 2;
            var heso = heSo(n - 1);
            var s = heso[0];
            var d = heso[1];
            long[] ran = { 2, 3, 5, 7, 23, 11, 17, 61 };
            bool laSoNT = true;
            foreach (long e in ran)
            {
                if (checkMillerRabin(s, d, n, e) == false) //\thuật toán ktra số nguyên tố cho những số lớn
                    laSoNT = false;
            }
            return laSoNT;
        }

        private void btnChonLai_Click(object sender, EventArgs e)
        {
            txtp.ResetText();
            txta.ResetText();
            txtd.ResetText();
            txtx.ResetText();
            txtk.ResetText();
            txty.ResetText();
        }

        private BigInteger LongRandom(BigInteger min, BigInteger max, Random rand)
        {
            byte[] bytes = min.ToByteArray();
            BigInteger R;

            rand.NextBytes(bytes);
            long n = 0;
            for (long i = 0; i < 1000000; i++)
            {
                n += 1;
            }
            bytes[bytes.Length - 1] &= (byte)0x7F; //buộc bit dấu thành dương
            R = new BigInteger(bytes);

            if (R < 0) R = -R;
            return R % max;

        }

        private BigInteger ChonSoNgauNhien(BigInteger min, BigInteger max)
        {
            BigInteger r = LongRandom(min, max, new Random());
            return r;
        }

      /*  private void btnKiemTraChuKy_Click(object sender, EventArgs e)
        {
            try
            {
                string digest;
                using (Stream stream = new FileStream(txtFileKiemTraChuKySo.Text, FileMode.Open))
                {
                    SHA256Managed sha256Managed = new SHA256Managed();
                    byte[] hashBytes = sha256Managed.ComputeHash(stream);
                    digest = Convert.ToBase64String(hashBytes);
                }
                BigInteger[] y2Moi = MaHoa(digest);
                BigInteger y1Cu;
                BigInteger[] y2Cu;
                using (StreamReader streamReader = new StreamReader(txtFileChuKy.Text))
                {
                    y1Cu = BigInteger.Parse(streamReader.ReadLine());
                    int yen = int.Parse(streamReader.ReadLine());
                    y2Cu = new BigInteger[yen];
                    for(int i=0; i < yen; i++)
                    {
                        y2Cu[i] = BigInteger.Parse(streamReader.ReadLine());
                    }
                    for (int i = 0; i < yen; i++)
                    {
                        if (y2Moi[i] != y2Cu[i])
                        {
                            MessageBox.Show("Tài liệu gửi đến đã bị thay đổi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }    
                         MessageBox.Show("Tài liệu gửi đến không bị chỉnh sửa gì", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show("Lỗi trong khi đọc file", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/

        private void tabPageThongTinTacGia_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void txtp_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtk_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtd_TextChanged(object sender, EventArgs e)
        {

        }


        private void txtChuKyBaoVanBan_TextChanged(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void txty_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtx_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog.InitialDirectory = "All files (*.*)|*.*|All files(*.*) | *.* ";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader read = new StreamReader(openFileDialog.FileName);
                txtNhan.Text = read.ReadToEnd();
                read.Close();
                txtKiemtra.Text = openFileDialog.FileName;
                string pathSignature = Path.ChangeExtension(openFileDialog.FileName, "sig");
                if (File.Exists(pathSignature))
                {
                    openFileDialog.FileName = pathSignature;
                    txtNhan1.Text = pathSignature;
                }
            }
        }

        private void txtFileKiemTraChuKySo_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string digest;
                using (Stream stream = new FileStream(txtKiemtra.Text, FileMode.Open))
                {
                    SHA256Managed sha256Managed = new SHA256Managed();
                    byte[] hashBytes = sha256Managed.ComputeHash(stream);
                    digest = Convert.ToBase64String(hashBytes);
                }
                BigInteger[] y2Moi = MaHoa(digest);
                BigInteger y1Cu;
                BigInteger[] y2Cu;
                using (StreamReader streamReader = new StreamReader(txtNhan1.Text))
                {
                    y1Cu = BigInteger.Parse(streamReader.ReadLine());
                    int yen = int.Parse(streamReader.ReadLine());
                    y2Cu = new BigInteger[yen];
                    for (int i = 0; i < yen; i++)
                    {
                        y2Cu[i] = BigInteger.Parse(streamReader.ReadLine());
                    }
                    for (int i = 0; i < yen; i++)
                    {
                        if (y2Moi[i] != y2Cu[i])
                        {
                            MessageBox.Show("Tài liệu gửi đến đã bị thay đổi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    MessageBox.Show("Tài liệu gửi đến không bị chỉnh sửa gì", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show("Lỗi trong khi đọc file", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = txtFileThucHienKy.Text;
          //  richTextBox1.Copy();
           // richTextBox2.Paste();
        }
        SaveFileDialog save;
        private void btnNhap_Click(object sender, EventArgs e)
        {
            save = new SaveFileDialog();
            save.Filter = "|.txt*";
            if(save.ShowDialog () == DialogResult.OK)
            {
                StreamWriter write = new StreamWriter(save.FileName);
                write.WriteLine(txtFileThucHienKy.Text);
                write.Close();
            }
            
           
        }

        private BigInteger[] MaHoa(string chuoiVao)
        {
            //Chuyen xâu thanh ma Unicode         

            byte[] mhE_temp1 = Encoding.Unicode.GetBytes(chuoiVao);
            string base64 = Convert.ToBase64String(mhE_temp1);

            // Chuyển xâu thành mã Unicode dạng số          
            BigInteger[] mh_temp2 = new BigInteger[base64.Length];
            for (int i = 0; i < base64.Length; i++)
            {
                mh_temp2[i] = (BigInteger)base64[i];
                //txtm1.Text += mh_temp2[i].ToString() + "#";
            }

            //txt_ChuoimaBanRo.Text = chuoi(mh_temp2);            
            //Mảng a chứa các kí tự sẽ  mã hóa
            BigInteger[] mh_temp3 = new BigInteger[mh_temp2.Length];
            // thực hiện mã hóa: z = (d^k * m ) mod p

            for (int i = 0; i < mh_temp2.Length; i++)
            {
                mh_temp3[i] = ((mh_temp2[i] % p) * (BinhPhuong(d, k, p))) % p;
                //txtm2.Text += mh_temp3[i].ToString()+"#";
            }
            return mh_temp3;
        }

        private void btnKyLenVanBan_Click(object sender, EventArgs e)
        {
            string digest = txtFileThucHienKy.Text;
            if (string.IsNullOrEmpty(txty.Text))
            {
                MessageBox.Show("Bạn cần nhập đủ thông tin", "Thông báo");
                return;
            }
            try
            {
                string path = Path.ChangeExtension(openFileDialog.FileName, "sig");
                if (File.Exists(path))
                {
                    if (MessageBox.Show("File: " + Path.GetFileName(path) + " đã có. Có muốn ghi đè?", "File đã có", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        return;
                    }
                }
                    using (StreamWriter streamWrite = File.CreateText(path))
                    {
                        streamWrite.WriteLine(y);
                        BigInteger[] hihi = MaHoa(digest);
                        streamWrite.WriteLine(hihi.Length);
                        foreach (BigInteger i in hihi)
                        {
                            streamWrite.WriteLine(i);
                        }

                    }
                MessageBox.Show("Đã ký thành công chữ kí ElGamal !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Lỗi", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private BigInteger sqr(BigInteger x)
        {
            return  x*x;  
        }

        private BigInteger BinhPhuong(BigInteger CoSo, BigInteger SoMu, BigInteger m)
        {
            if (SoMu == 0) return 1 % m;
            else
                if (SoMu % 2 == 0)
                return  (sqr(BinhPhuong(CoSo, SoMu / 2, m)) % m);
            else
                return (CoSo * (((sqr(BinhPhuong(CoSo, SoMu / 2, m)) % m)) %  m) % m);
        }

        private void TaoKhoa()
        {
            a = d = x = k = y = 0;
            do
            {
                //tim so a (phan tu nguyen thuy)
                a = ChonSoNgauNhien(p / 2, p - 1);
                //tim khoa bi mat x
                x = ChonSoNgauNhien(p / 2, p - 2);
                //tim k
                k = ChonSoNgauNhien(p / 2, p - 1);
            } while (a == x || x == k || k == a );
            
            //d=a^x mod p
            d = BinhPhuong(a, x, p);
            
            //y=a^k mod p
            y = BinhPhuong(a, k, p);
        }



        private void btnTaoKhoaNgauNhien_Click(object sender, EventArgs e)
        {
            p = 0;
            do
            {
                p = ChonSoNgauNhien(10000000000000000000, 10000000000000000100);
            }
            while (KiemTraSoNguyenTo(p) == false);
            TaoKhoa();
            txtp.Text = p.ToString();
            txta.Text = a.ToString();
            txtd.Text = d.ToString();
            txtx.Text = x.ToString();
            txtk.Text = k.ToString();
            txty.Text = y.ToString();
        }
    }
}
