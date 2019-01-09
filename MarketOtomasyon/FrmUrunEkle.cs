﻿using MarketOtomasyon.BLL.Repositories;
using MarketOtomasyon.Models.Entities;
using MarketOtomasyon.Models.ViewModels;
using MarketOtomasyon.Models.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarketOtomasyon
{
    public partial class FrmUrunEkle : Form
    {
        public FrmUrunEkle()
        {
            InitializeComponent();
        }



        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            List<Category> categories = new CategoryRepo().GetAll();
            bool varMi = false;
            try
            {
                if (categories.Count == 0)
                {
                    new CategoryRepo().Insert(new Category()
                    {
                        CategoryName = txtCategory.Text,
                        KdvRate = nuKDV.Value
                    });
                    MessageBox.Show("Kategori ekleme islemi basarili");
                }
                else
                {

                    foreach (var cat in categories)
                    {
                        if (cat.CategoryName == txtCategory.Text)
                            varMi = true;
                    }

                    if (!varMi)
                    {
                        new CategoryRepo().Insert(new Category()
                        {
                            CategoryName = txtCategory.Text,
                            KdvRate = nuKDV.Value
                        });
                        MessageBox.Show("Kategori ekleme islemi basarili");
                    }
                    else
                        throw new Exception("Bu isimde bir kategori bulunmaktadir");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            GetCategories();
        }
        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            if (txtProduct.Text == null || cmbCategory.SelectedItem == null || txtBarcode.Text == null || txtSellPrice.Text == null) return;

            List<Product> products = new ProductRepo().GetAll();
            try
            {
                Product product = new Product()
                {
                    CategoryId = (cmbCategory.SelectedItem as CmbCategoryViewModel).Id,
                    ProductName = txtProduct.Text,
                    Barcode = txtBarcode.Text,
                    SellPrice = Convert.ToDecimal(txtSellPrice.Text)
                };

                using (var productRepo = new ProductRepo())
                {
                    foreach (var item in products)
                    {
                        if (product.Barcode == item.Barcode) throw new Exception("Aynı barkoda sahip ürününz var");
                        if (item.CategoryId == product.CategoryId && item.ProductName == product.ProductName) throw new Exception("Bu kategoride bu isimde bir ürün bulunmaktadir");
                    }
                    productRepo.Insert(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void GetCategories()
        {
            var categories = new CategoryRepo().GetAll()
                                               .OrderBy(x => x.CategoryName)
                                               .Select(x => new CategoryViewModel()
                                               {
                                                   Id = x.Id,
                                                   Name = x.CategoryName,
                                                   KdvRate = x.KdvRate

                                               }).ToList();

            lstCategories.DataSource = categories;

        }

        private void FrmUrunEkle_Load_1(object sender, EventArgs e)
        {
            List<CmbCategoryViewModel> categories = new List<CmbCategoryViewModel>();
            List<ProductViewModel> products = new List<ProductViewModel>();
            try
            {
                products.AddRange(new ProductRepo().GetAll()
                    .OrderBy(x => x.ProductName)
                    .Select(x => new ProductViewModel()
                    {
                        Id = x.Id,
                        ProductName = x.ProductName,
                        SellPrice = x.SellPrice,
                        StockQuantity = x.StockQuantity,
                        Barcode = x.Barcode
                    }));
                categories.AddRange(new CategoryRepo().GetAll()
                    .OrderBy(x => x.CategoryName)
                    .Select(x => new CmbCategoryViewModel()
                    {
                        Id = x.Id,
                        CategoryName = x.CategoryName
                    }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            cmbCategory.DataSource = categories;
            lstCategories.DataSource = categories;
            lstProducts.DataSource = products;
            GetCategories();
        }
    }
}
