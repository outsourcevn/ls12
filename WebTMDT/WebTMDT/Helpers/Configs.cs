using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using WebTMDT.Models;

namespace WebTMDT.Helpers
{
    public static class Configs
    {
        public const int ImageMinimumBytes = 512;
        public static bool IsImage(HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (System.IO.Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && System.IO.Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && System.IO.Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && System.IO.Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }

                if (postedFile.ContentLength < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[512];
                postedFile.InputStream.Read(buffer, 0, 512);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static string unicodeToNoMark(string input)
        {
            input = input.ToLowerInvariant().Trim();
            if (input == null) return "";
            string noMark = "a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,e,e,e,e,e,e,e,e,e,e,e,e,u,u,u,u,u,u,u,u,u,u,u,u,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,i,i,i,i,i,i,y,y,y,y,y,y,d,A,A,E,U,O,O,D";
            string unicode = "a,á,à,ả,ã,ạ,â,ấ,ầ,ẩ,ẫ,ậ,ă,ắ,ằ,ẳ,ẵ,ặ,e,é,è,ẻ,ẽ,ẹ,ê,ế,ề,ể,ễ,ệ,u,ú,ù,ủ,ũ,ụ,ư,ứ,ừ,ử,ữ,ự,o,ó,ò,ỏ,õ,ọ,ơ,ớ,ờ,ở,ỡ,ợ,ô,ố,ồ,ổ,ỗ,ộ,i,í,ì,ỉ,ĩ,ị,y,ý,ỳ,ỷ,ỹ,ỵ,đ,Â,Ă,Ê,Ư,Ơ,Ô,Đ";
            string[] a_n = noMark.Split(',');
            string[] a_u = unicode.Split(',');
            for (int i = 0; i < a_n.Length; i++)
            {
                input = input.Replace(a_u[i], a_n[i]);
            }
            input = input.Replace("  ", " ");
            input = Regex.Replace(input, "[^a-zA-Z0-9% ._]", string.Empty);
            input = removeSpecialChar(input);
            input = input.Replace(" ", "-");
            input = input.Replace("--", "-");
            return input;
        }

        public static string removeSpecialChar(string input)
        {
            input = input.Replace("-", "").Replace(":", "").Replace(",", "").Replace("_", "").Replace("'", "").Replace("\"", "").Replace(";", "").Replace("”", "").Replace(".", "").Replace("%", "");
            return input;
        }

        public static string splitString(string input)
        {
            string strNew = unicodeToNoMark(input);
            return strNew.Replace("-", " ");
        }

        public static List<sortOrder> KhoangGia()
        {
            var numbersArray = Enumerable.Range(1, 20).ToArray();
            List<int> List_int1 = new List<int>();
            for (int i = 1; i <= numbersArray.Length; i += 2)
            {
                List_int1.Add(i);
            }
            List_int1.Add(20);

            List<sortOrder> myList = new List<sortOrder>();

            for (int i = 0; i < List_int1.Count; i++)
            {
                if (i == 10)
                {
                    break;
                }
                var _x = string.Format("{0}-{1}", List_int1[i], List_int1[i + 1]);
                CultureInfo vi_VN = new CultureInfo("vi-VN");
                var _y = string.Format(vi_VN.NumberFormat, "Từ {0:n0}- đến {1:n0}", List_int1[i] * 1000000, List_int1[i + 1] * 1000000);
                myList.Add(new sortOrder() { NameSort = _y, TypeSort = _x });
            }
            return myList;
        }

        public class TwoNumber
        {
            public int Number1 { get; set; }
            public int Number2 { get; set; }
        }

        public static TwoNumber GetNumber(string textNumber)
        {
            // textNumber = "3-5";
            string[] x = textNumber.Split('-');
            TwoNumber _tw = new TwoNumber();
            _tw.Number1 = int.Parse(x[0]) * 1000000;
            _tw.Number2 = int.Parse(x[1]) * 1000000;
            return _tw;
        }

    }
}