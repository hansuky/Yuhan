using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Win32;
using Yuhan.Common.Helpers;
using Yuhan.WPF.Commands;

namespace Yuhan.WPF.Login.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private String userId;
        /// <summary>
        /// 로그인할 사용자의 아이디를 나타냅니다.
        /// </summary>
        public String UserId
        {
            get { return userId; }
            set
            {
                userId = value;
                OnPropertyChanged("UserId");
            }
        }

        private String userPassword;
        /// <summary>
        /// 로그인할 사용자의 비밀번호를 나타냅니다.
        /// </summary>
        public String UserPassword
        {
            get { return userPassword; }
            set
            {
                userPassword = value;
                OnPropertyChanged("UserPassword");
            }
        }

        private IEnumerable<String> categories;
        /// <summary>
        /// 로그인시 사용돼는 카테고리의 리스트를 나타냅니다.
        /// </summary>
        public IEnumerable<String> Categories
        {
            get { return categories; }
            set
            {
                categories = value;
                OnPropertyChanged("Categories");
            }
        }

        private String selectedCategory;
        /// <summary>
        /// 로그인시 사용됄 선택됀 카테고리를 나타냅니다.
        /// </summary>
        public String SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                OnPropertyChanged("SelectedCategory");
            }
        }

        private Boolean isLoaded;
        /// <summary>
        /// 로그인에 필요한 데이터가 서버로 부터 모두 로드됐는지의 여부를 나타냅니다.
        /// </summary>
        protected Boolean IsLoaded
        {
            get { return isLoaded; }
            set { isLoaded = value; }
        }

        public LoginViewModel()
        {
            this.IsLoaded = false;
        }

        private ICommand loginCommand;
        /// <summary>
        /// Login Command 객체 입니다.
        /// </summary>
        public ICommand LoginCommand
        {
            get
            {
                if (loginCommand == null)
                    loginCommand = new RelayCommand(Login, CanLogin);
                return loginCommand;
            }
        }

        /// <summary>
        /// 현제 데이터에서 Login이 가능한지의 여부를 나타냅니다.
        /// </summary>
        /// <returns></returns>
        public Boolean CanLogin()
        {
            return IsValid;
        }

        /// <summary>
        /// 사용자 아이디, 패스워드 및 카테고리를 사용해 로그인을 시도합니다.
        /// </summary>
        public virtual void Login()
        {
            try
            {
                //LString[] sLogInParameters = new LString[] { new LString(this.UserId.Trim()), new LString(this.UserPassword.Trim()) };
                //DataTable AuthorityFunctionList = AuthorityLogInHandler.SelectByCondition(sLogInParameters);

                //RegistryKey _RegistryKey = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("iMES");
                //_RegistryKey.SetValue("Category", this.SelectedCategory.ToString().Trim());
                //_RegistryKey.SetValue("UserID", this.UserId.Trim());
                
                //로그인 실패시 Throw Exception
                
                OnLogined(this.UserId);
            }
            catch (Exception ex)
            {
                OnLogined(this.UserId, false, ex);
            }
        }
        
        /// <summary>
        /// 로그인에 필요한 데이터를 모두 로드 합니다.
        /// </summary>
        public void Load()
        {
            if (!this.IsLoaded)
                Categories = GetCategories();
        }

        /// <summary>
        /// 로그인에 필요한 데이터를 비동기로 모두 로드 합니다.
        /// </summary>
        public void LoadAsync()
        {
            LoadWorker.RunWorkerAsync();
        }

        /// <summary>
        /// 로그인 시도 후 발생합니다.
        /// </summary>
        public event LoginEventHandler Logined;
        /// <summary>
        /// 로그인 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="userId"></param>
        protected void OnLogined(String userId, Boolean isSuccess = true, Exception ex = null)
        {
            if (Logined != null)
                Logined(this, new LoginEventArgs(userId, isSuccess, ex));
        }

        private BackgroundWorker loadWorker;
        /// <summary>
        /// 로그인에 필요한 데이터를 모두 로드시키는 비동기 스레드 개체입니다.
        /// </summary>
        private BackgroundWorker LoadWorker
        {
            get
            {
                if (loadWorker == null)
                {
                    loadWorker = new BackgroundWorker();
                    loadWorker.DoWork += (sender, e) => { Load(); };
                    loadWorker.RunWorkerCompleted += (sender, e) => { IsLoaded = true; };
                }
                return loadWorker;
            }
            set { loadWorker = value; }
        }

        /// <summary>
        /// 로그인의 카테고리 목록을 서버로 부터 로드합니다.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<String> GetCategories()
        {
            yield return "Category 1";
            yield return "Category 2";
            yield return "Category 3";
            yield return "Category 4";
            yield return "Category 5";
        }

        /// <summary>
        /// 현재 서식이 로그인이 가능한 상태인지를 나타냅니다.
        /// </summary>
        public Boolean IsValid
        {
            get { return GetRuleViolations().Count() == 0 ? true : false; }
        }

        /// <summary>
        /// 현재 서식의 유효성 검사 결과 리스트입니다.
        /// </summary>
        /// <returns>유효성 검사 결과 리스트</returns>
        public IEnumerable<RuleViolation> GetRuleViolations()
        {
            if (String.IsNullOrEmpty(this.SelectedCategory))
                yield return new RuleViolation("카테고리는 필수 값입니다.", SelectedCategory);

            foreach (var rule in GetUserIDRuleViolations())
                yield return rule;

            foreach (var rule in GetUserPasswordRuleViolations())
                yield return rule;
        }

        /// <summary>
        /// 현재 서식의 로그인 아이디 유효성 검사 결과 리스트입니다.
        /// </summary>
        /// <returns>로그인 아이디 유효성 검사 결과 리스트</returns>
        public IEnumerable<RuleViolation> GetUserIDRuleViolations()
        {
            if (String.IsNullOrEmpty(this.UserId))
                yield return new RuleViolation("아이디는 필수 값입니다.", "UserId");
            else if(this.UserId.Any(s => Char.IsWhiteSpace(s)))
                yield return new RuleViolation("아이디는 공백을 포함할 수 없습니다.", "UserId");
        }

        /// <summary>
        /// 현재 서식의 로그인 비밀번호 유효성 검사 결과 리스트입니다.
        /// </summary>
        /// <returns>로그인 비밀번호 유효성 검사 결과 리스트</returns>
        public IEnumerable<RuleViolation> GetUserPasswordRuleViolations()
        {
            if (String.IsNullOrEmpty(this.UserPassword))
                yield return new RuleViolation("비밀번호는 필수 값입니다.", "UserPassword");
        }

        #region Implementation INotifyPropertyChanged
        protected void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Implementation IDataError
        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// Not used by WPF.
        /// </summary>
        /// <returns>
        /// An error message indicating what is wrong with this object. The default is an empty string ("").
        /// </returns>
        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get { return this.Validate(columnName); }
        }

        private string Validate(string propertyName)
        {
            switch (propertyName)
            {
                case "UserId":
                    return this.GetUserIDRuleViolations().Count() > 0 ? this.GetUserIDRuleViolations().First().ErrorMessage : null;

                case "UserPassword":
                    return this.GetUserPasswordRuleViolations().Count() > 0 ? this.GetUserPasswordRuleViolations().First().ErrorMessage : null;

                default:
                    // return valid by default
                    return null;
            }
        }
        #endregion
    }

    #region Login Events
    public delegate void LoginEventHandler(object sender, LoginEventArgs e);

    public class LoginEventArgs : EventArgs
    {
        private String userId;
        /// <summary>
        /// 로그인됀 사용자 아이디를 나타냅니다.
        /// </summary>
        public String UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        private Boolean success;
        /// <summary>
        /// 로그인 성공 여부를 나타냅니다.
        /// </summary>
        public Boolean Success
        {
            get { return success; }
            set { success = value; }
        }

        private Exception ex;
        /// <summary>
        /// 로그인 실패시 나타난 예외입니다.
        /// </summary>
        public Exception Exception
        {
            get { return ex; }
            set { ex = value; }
        }

        public LoginEventArgs(String userId, Boolean success = true, Exception ex = null)
        {
            this.UserId = userId;
            this.Success = success;
            this.Exception = ex;
        }
    }
    #endregion
}
