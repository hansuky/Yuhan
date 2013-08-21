using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Yuhan.WPF.MessageBox
{
    public class MessageBoxViewModel : INotifyPropertyChanged
    {
        private MessageBoxWindowResult result;
        /// <summary>
        /// 메세지 박스의 결과를 나타냅니다.
        /// </summary>
        public MessageBoxWindowResult Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged("Result");
            }
        }

        private String caption;
        /// <summary>
        /// 메세지 박스의 캡션[제목]을 나타냅니다.
        /// </summary>
        public String Caption
        {
            get { return caption; }
            set
            {
                caption = value;
                OnPropertyChanged("Caption");
            }
        }

        private String header;
        /// <summary>
        /// 머리글을 나타냅니다.
        /// </summary>
        public String Header
        {
            get { return header; }
            set
            {
                header = value;
                OnPropertyChanged("Header");
            }
        }

        private MessageBoxWindowIcons? headerIcon;
        /// <summary>
        /// 머리글에 사용할 아이콘입니다.
        /// </summary>
        public MessageBoxWindowIcons? HeaderIcon
        {
            get { return headerIcon; }
            set
            {
                headerIcon = value;
                OnPropertyChanged("HeaderIcon");
            }
        }

        private String description;
        /// <summary>
        /// 설명글을 나타냅니다.
        /// </summary>
        public String Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        private String details;
        /// <summary>
        /// 상세 설명글을 나타냅니다.
        /// </summary>
        public String Details
        {
            get { return details; }
            set
            {
                details = value;
                OnPropertyChanged("Details");
            }
        }

        private String footerText;
        /// <summary>
        /// 하단 텍스트를 나타냅니다.
        /// </summary>
        public String FooterText
        {
            get { return footerText; }
            set
            {
                footerText = value;
                OnPropertyChanged("FooterText");
            }
        }

        private int? percentage;
        /// <summary>
        /// 처리 완료 수치를 나타냅니다.
        /// </summary>
        public int? Percentage
        {
            get { return percentage; }
            set
            {
                percentage = value;
                OnPropertyChanged("Percentage");
            }
        }


        private MessageBoxWindowIcons? footerIcon;
        /// <summary>
        /// 하단에 사용할 아이콘입니다.
        /// </summary>
        public MessageBoxWindowIcons? FooterIcon
        {
            get { return footerIcon; }
            set
            {
                footerIcon = value;
                OnPropertyChanged("FooterIcon");
            }
        }


        private MessageBoxWindowButtons buttons;
        /// <summary>
        /// 메세지 박스에 사용할 버튼을 나타냅니다.
        /// </summary>
        public MessageBoxWindowButtons Buttons
        {
            get { return buttons; }
            set
            {
                buttons = value;
                OnPropertyChanged("Buttons");
            }
        }

        private Boolean isDialog;
        public Boolean IsDialog
        {
            get { return isDialog; }
            set
            {
                isDialog = value;
                OnPropertyChanged("IsDialog");
            }
        }


        public MessageBoxViewModel()
        {
            #region Sample Data
            //this.Caption = "Caption[Sample]";
            //this.Header = "Instruction Heading[Sample]";
            //this.HeaderIcon = MessageBoxWindowIcons.Information;
            //this.Description = "Instruction[Sample]";
            //this.Details = "Additional Details Text[Sample]";
            //this.FooterText = "FooterText[Sample]";
            //this.FooterIcon = MessageBoxWindowIcons.Shield;
            #endregion
            this.Result = MessageBoxWindowResult.None;
            IsDialog = true;
            this.FooterIcon = MessageBoxWindowIcons.Information;
            this.FooterText = "ITComm";
        }

        #region Implementaion INotifyPropertyChanged
        protected void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
