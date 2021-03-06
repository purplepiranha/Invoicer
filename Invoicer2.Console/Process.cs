﻿using Invoicer2.Models;
using Invoicer2.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Invoicer2.Console
{
    public class Process
    {
        public void Go()
        {
            this.GenerateTestWithoutVAT();
            //this.GenerateTestWithoutVATAndWithDiscount();
            //this.GenerateTestWithVat();
            //this.GenerateTestWithVatAndDiscount();
        }

        /// <summary>
        /// Generates the test without VAT or discount.
        /// </summary>
        public void GenerateTestWithoutVAT()
        {
            byte[] image = LoadImage("vodafone.jpg");
            string imageFilename = MigraDocFilenameFromByteArray(image);

            var invoice = new InvoicerApi(SizeOption.A4, OrientationOption.Landscape, "£")
            .TextColor("#CC0000")
            .BackColor("#FFD6CC")
            .Image(imageFilename, 125, 27)
            .Company(Address.Make(
                "FROM",
                new string[] {
                    "Vodafone Limited",
                    "Vodafone House",
                    "The Connection",
                    "Newbury",
                    "Berkshire RG14 2FN"
                },
                "1471587",
                null,
                new string[] {
                    "Vodafone Limited. Registered in England and Wales No. 1471587.",
                    "Registered office: Vodafone House, The Connection, Newbury, Berkshire RG14 2FN."
                }))
            .Client(Address.Make("INVOICE TO", new string[] { "Isabella Marsh", "Overton Circle", "Little Welland", "Worcester", "WR## 2DJ" }))
            .Items(new List<ItemRow> {
                ItemRow.Make("Nexus 6", "Midnight Blue", (decimal)1, (decimal)199.99, (decimal)199.99),
                ItemRow.Make("24 Months (£22.50pm)", "100 minutes, Unlimited texts, 100 MB data 3G plan with 3GB of UK Wi-Fi", (decimal)1, (decimal)432.00, (decimal)432.00),
                ItemRow.Make("Special Offer", "Free case (blue)", (decimal)1, (decimal)0, (decimal)0),
            })
            .Totals(new List<TotalRow> {
                TotalRow.Make("Total", (decimal)631.99, true),
            })
            .Details(new List<DetailRow> {
                DetailRow.Make("PAYMENT INFORMATION", "Make all cheques payable to Vodafone UK Limited.", "", "If you have any questions concerning this invoice, contact our sales department at sales@vodafone.co.uk.", "", "Thank you for your business.")
            })
            .Footer("http://www.vodafone.co.uk");

            using (var stream = invoice.Get())
            {
                using (var fs = new FileStream("fs.pdf", FileMode.Create))
                {
                    stream.CopyTo(fs);
                }
            }


        }

        static string MigraDocFilenameFromByteArray(byte[] image)
        {
            return "base64:" +
                   Convert.ToBase64String(image);
        }

        /// <summary>
        /// Generates the test with discount and without VAT.
        /// </summary>
        public void GenerateTestWithoutVATAndWithDiscount()
        {
            new InvoicerApi(SizeOption.A4, OrientationOption.Landscape, "£")
                .TextColor("#CC0000")
                .BackColor("#FFD6CC")
                .Image(@"..\..\..\images\vodafone.jpg", 125, 27)
                .Company(Address.Make("FROM", new string[] { "Vodafone Limited", "Vodafone House", "The Connection", "Newbury", "Berkshire RG14 2FN" }, "1471587", null))
                .Client(Address.Make("BILLING TO", new string[] { "Isabella Marsh", "Overton Circle", "Little Welland", "Worcester", "WR## 2DJ" }))
                .Items(new List<ItemRow> {
                    ItemRow.Make("Nexus 6", "Midnight Blue", (decimal)1, 0, (decimal)199.99, (decimal)199.99),
                    ItemRow.Make("24 Months (£22.50pm)", "100 minutes, Unlimited texts, 100 MB data 3G plan with 3GB of UK Wi-Fi", (decimal)1, (decimal)432.00, (decimal)432.00),
                    ItemRow.Make("Special Offer", "Free case (blue)", (decimal)1, (decimal)0, (decimal)0),
                    ItemRow.Make("Test", "This needs improving", (decimal)1, (decimal)10, "-5.00", (decimal)-5),
                })
                .Totals(new List<TotalRow> {
                    TotalRow.Make("Total", (decimal)626.99, true),
                })
                .Details(new List<DetailRow> {
                    DetailRow.Make("PAYMENT INFORMATION", "Make all cheques payable to Vodafone UK Limited.", "", "If you have any questions concerning this invoice, contact our sales department at sales@vodafone.co.uk.", "", "Thank you for your business.")
                })
                .Footer("http://www.vodafone.co.uk")
                .Save("NonVAT_Discount.pdf");
        }

        /// <summary>
        /// Generates the test with VAT.
        /// </summary>
        public void GenerateTestWithVat()
        {
            new InvoicerApi(SizeOption.A4, OrientationOption.Landscape, "£")
                .TextColor("#CC0000")
                .BackColor("#FFD6CC")
                .Image(@"..\..\..\images\vodafone.jpg", 125, 27)
                .Company(Address.Make("FROM", new string[] { "Vodafone Limited", "Vodafone House", "The Connection", "Newbury", "Berkshire RG14 2FN" }, "1471587", "569953277"))
                .Client(Address.Make("BILLING TO", new string[] { "Isabella Marsh", "Overton Circle", "Little Welland", "Worcester", "WR## 2DJ" }))
                .Items(new List<ItemRow> {
                    ItemRow.Make("Nexus 6", "Midnight Blue", (decimal)1, 20, (decimal)166.66, (decimal)199.99),
                    ItemRow.Make("24 Months (£22.50pm)", "100 minutes, Unlimited texts, 100 MB data 3G plan with 3GB of UK Wi-Fi", (decimal)1, 20, (decimal)360.00, (decimal)432.00),
                    ItemRow.Make("Special Offer", "Free case (blue)", (decimal)1, 0, (decimal)0, (decimal)0),
                })
                .Totals(new List<TotalRow> {
                    TotalRow.Make("Sub Total", (decimal)526.66),
                    TotalRow.Make("VAT @ 20%", (decimal)105.33),
                    TotalRow.Make("Total", (decimal)631.99, true),
                })
                .Details(new List<DetailRow> {
                    DetailRow.Make("PAYMENT INFORMATION", "Make all cheques payable to Vodafone UK Limited.", "", "If you have any questions concerning this invoice, contact our sales department at sales@vodafone.co.uk.", "", "Thank you for your business.")
                })
                .Footer("http://www.vodafone.co.uk")
                .Save("VAT.pdf");
        }

        /// <summary>
        /// Generates the test with VAT and discount.
        /// </summary>
        public void GenerateTestWithVatAndDiscount()
        {
            new InvoicerApi(SizeOption.A4, OrientationOption.Landscape, "£")
                .TextColor("#CC0000")
                .BackColor("#FFD6CC")
                .Image(@"..\..\..\images\vodafone.jpg", 125, 27)
                .Company(Address.Make("FROM", new string[] { "Vodafone Limited", "Vodafone House", "The Connection", "Newbury", "Berkshire RG14 2FN" }, "1471587", "569953277"))
                .Client(Address.Make("BILLING TO", new string[] { "Isabella Marsh", "Overton Circle", "Little Welland", "Worcester", "WR## 2DJ" }))
                .Items(new List<ItemRow> {
                    ItemRow.Make("Nexus 6", "Midnight Blue", (decimal)1, 20, (decimal)166.66, (decimal)199.99),
                    ItemRow.Make("24 Months (£22.50pm)", "100 minutes, Unlimited texts, 100 MB data 3G plan with 3GB of UK Wi-Fi", (decimal)1, 20, (decimal)360.00, (decimal)432.00),
                    ItemRow.Make("Special Offer", "Free case (blue)", (decimal)1, 0, (decimal)0, (decimal)0),
                    ItemRow.Make("Test", "This needs improving", (decimal)1, 0, (decimal)10, "-5.00", (decimal)-5),
                })
                .Totals(new List<TotalRow> {
                    TotalRow.Make("Sub Total", (decimal)526.66),
                    TotalRow.Make("VAT @ 20%", (decimal)105.33),
                    TotalRow.Make("Total", (decimal)631.99, true),
                })
                .Details(new List<DetailRow> {
                    DetailRow.Make("PAYMENT INFORMATION", "Make all cheques payable to Vodafone UK Limited.", "", "If you have any questions concerning this invoice, contact our sales department at sales@vodafone.co.uk.", "", "Thank you for your business.")
                })
                .Footer("http://www.vodafone.co.uk")
                .Save("VAT_Discount.pdf");
        }

        static byte[] LoadImage(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("Invoicer2.Console.images."+name))
            {
                if (stream == null)
                    throw new ArgumentException("No resource with name " + name);

                int count = (int)stream.Length;
                byte[] data = new byte[count];
                stream.Read(data, 0, count);
                return data;
            }
        }

    }
}
