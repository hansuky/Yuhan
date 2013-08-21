using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Media;

namespace Yuhan.WPF.DsxGridCtrl.Demo
{
    public class Customer : INotifyPropertyChanged
    {
        #region events

        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion

        #region ctors

        public Customer(XElement xElement)
        {
            this.CustomerID     = ExtractXString(xElement, "CustomerID");
            this.CompanyName    = ExtractXString(xElement, "CompanyName");
            this.ContactName    = ExtractXString(xElement, "ContactName");
            this.ContactTitle   = ExtractXString(xElement, "ContactTitle");
            this.Address        = ExtractXString(xElement, "Address");
            this.City           = ExtractXString(xElement, "City");
            this.PostalCode     = ExtractXString(xElement, "PostalCode");
            this.Country        = ExtractXString(xElement, "Country");
            this.Phone          = ExtractXString(xElement, "Phone");
            this.Fax            = ExtractXString(xElement, "Fax");
            this.DateRegistered = ExtractXDate  (xElement, "DateRegistered");

            this.Coverage       = ExtractXDecimal(xElement, "Coverage");
            this.Sales          = ExtractXDecimal(xElement, "Sales");
        }
        #endregion

        #region Method - ExtractXString

        private string ExtractXString(XElement xElement, string propertyName)
		{
            if (xElement != null && xElement.Element(propertyName) != null)
            {
                return xElement.Element(propertyName).Value;
            }
            else
			{
				return String.Empty;
			}
		}
        #endregion

        #region Method - ExtractXDate

        private DateTime ExtractXDate(XElement xElement, string propertyName)
		{
            DateTime _result = new DateTime(0);

            if (xElement != null && xElement.Element(propertyName) != null)
            {
                DateTime.TryParse(xElement.Element(propertyName).Value, out _result);
            }
            return _result;
		}
        #endregion

        #region Method - ExtractXDecimal

        private decimal ExtractXDecimal(XElement xElement, string propertyName)
		{
            decimal _result = 0.0M;

            if (xElement != null && xElement.Element(propertyName) != null)
            {
                Decimal.TryParse(xElement.Element(propertyName).Value, out _result);
            }
            return _result;
		}
        #endregion


        #region Method - RaiseOnPropertyChanged

        private void RaiseOnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
        #endregion

        
        #region Property - MarkFlag

        private bool m_markFlag;

		public bool MarkFlag
		{
			get {   return m_markFlag;    }
			set {   if (m_markFlag != value)
    				{
    					m_markFlag = value;
    					RaiseOnPropertyChanged("MarkFlag");
    				}
    			}
		}
        #endregion


        #region Property - CustomerID

        private string m_customerID;

		public string CustomerID
		{
			get {   return m_customerID;    }
			set {   if (m_customerID != value)
    				{
    					m_customerID = value;
    					RaiseOnPropertyChanged("CustomerID");
    				}
    			}
		}
        #endregion

        #region Property - CompanyName

        private string m_companyName;

        public string CompanyName
		{
			get {   return m_companyName;   }
			set {   if (m_companyName != value)
    				{
    					m_companyName = value;
    					RaiseOnPropertyChanged("CompanyName");
    				}
    			}
		}
        #endregion

        #region Property - ContactName

        private string m_contactName;

		public string ContactName
		{
			get {   return m_contactName;   }
			set {   if (m_contactName != value)
    				{
    					m_contactName = value;
    					RaiseOnPropertyChanged("ContactName");
    				}
    			}
		}
        #endregion

        #region Property - ContactTitle

        private string m_contactTitle;

        public string ContactTitle
		{
			get {   return m_contactTitle;  }
			set {   if (m_contactTitle != value)
    				{
    					m_contactTitle = value;
    					RaiseOnPropertyChanged("ContactTitle");
    				}
    			}
		}
        #endregion

        #region Property - Address

        private string m_address;

		public string Address
		{   
            get {   return m_address;   }
			set {   if (m_address != value)
    				{
    					m_address = value;
    					RaiseOnPropertyChanged("Address");
    				}
    			}
		}
        #endregion

        #region Property - City

        private string m_city;

        public string City
		{
			get {   return m_city;  }
			set {   if (m_city != value)
    				{
    					m_city = value;
    					RaiseOnPropertyChanged("City");
    				}
    			}
		}
        #endregion

        #region Property - PostalCode

        private string m_postalCode;

        public string PostalCode
		{
			get {   return m_postalCode;    }
			set {   if (m_postalCode != value)
    				{
    					m_postalCode = value;
    					RaiseOnPropertyChanged("PostalCode");
    				}
    			}
		}
        #endregion

        #region Property - Country

        private string      m_country;

        public string Country
		{
			get {   return m_country;   }
			set {
    				if (m_country != value)
    				{
    					m_country = value;
    					RaiseOnPropertyChanged("Country");
    				}
    			}
		}
        #endregion

        #region Property - Phone

        private string m_phone;

		public string Phone
		{
			get {   return m_phone; }
			set {   if (m_phone != value)
    				{
    					m_phone = value;
    					RaiseOnPropertyChanged("Phone");
    				}
    			}
		}
        #endregion

        #region Property - Fax

		private string m_fax;

		public string Fax
		{
			get {   return m_fax;   }
			set {   if (m_fax != value)
    				{
    					m_fax = value;
    					RaiseOnPropertyChanged("Fax");
    				}
    			}
		}
        #endregion

        #region Property - DateRegistered

		private DateTime m_dateRegistred;

		public DateTime DateRegistered
		{
			get {   return m_dateRegistred;   }
			set {   if (m_dateRegistred != value)
    				{
    					m_dateRegistred = value;
    					RaiseOnPropertyChanged("DateRegistered");
    				}
    			}
		}
        #endregion


        #region Property - Coverage

		private decimal m_coverage;

		public decimal Coverage
		{
			get {   return m_coverage;   }
			set {   if (m_coverage != value)
    				{
    					m_coverage = value;
    					RaiseOnPropertyChanged("Coverage");
                        RaiseOnPropertyChanged("IsVip");
    				}
    			}
		}
        #endregion


        #region Property - Avatar

		public ImageSource Avatar
		{
            get 
            {
                ImageSource _result = App.Current.MainWindow.FindResource("imgForward") as ImageSource;
                return _result;
            }
		}
        #endregion

        #region Property - IsVip

        public bool IsVip
		{
			get {   return (m_coverage > 90.0M);    }
		}
        #endregion

        #region Property - Sales

        private decimal m_sales;

		public decimal Sales
		{
			get {   return m_sales;    }
			set {   if (m_sales != value)
    				{
    					m_sales = value;
    					RaiseOnPropertyChanged("Sales");
    				}
    			}
		}
        #endregion

    }
}
