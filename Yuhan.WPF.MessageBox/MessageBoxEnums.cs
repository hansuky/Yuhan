using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Yuhan.WPF
{
    /// <summary>
    /// 메세지 박스에 사용할 버튼의 열거형 오브젝트입니다.
    /// </summary>
    public enum MessageBoxWindowButtons
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    /// <summary>
    /// 메세지 박스에 사용할 아이콘의 열거형 오브젝트입니다.
    /// </summary>
    public enum MessageBoxWindowIcons
    {
        None,
        Information,
        Question,
        Shield,
        Stop,
        Warning
    }

    /// <summary>
    /// 메세지 박스에 사용할 결과의 열거형 오브젝트입니다.
    /// </summary>
    public enum MessageBoxWindowResult
    {
        None,
        OK,
        Cancel,
        Yes,
        No
    }
}
