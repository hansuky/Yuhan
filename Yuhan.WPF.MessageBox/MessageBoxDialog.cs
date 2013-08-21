using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Yuhan.WPF.MessageBox;

namespace Yuhan.WPF
{
    /// <summary>
    /// 텍스트, 단추 및 기호를 포함하는 메시지 상자를 표시하여 사용자에게 필요한 정보와 명령을 제공할 수 있습니다.
    /// </summary>
    public static class MessageBoxDialog
    {
        /// <summary>
        /// 지정된 텍스트가 있는 메시지 상자를 표시합니다.
        /// </summary>
        /// <param name="text">메시지 상자에 표시할 텍스트입니다.</param>
        /// <returns>Yuhan.WPF.MessageBox.MessageBoxWindowResult 값 중 하나입니다.</returns>
        public static MessageBoxWindowResult Show(String text)
        {
            return Show(new MessageBoxViewModel() { Header = text });
        }
        /// <summary>
        /// 지정된 텍스트와 캡션이 있는 메시지 상자를 표시합니다.
        /// </summary>
        /// <param name="text">메시지 상자에 표시할 텍스트입니다.</param>
        /// <param name="caption">메시지 상자의 제목 표시줄에 표시할 텍스트입니다.</param>
        /// <returns>Yuhan.WPF.MessageBox.MessageBoxWindowResult 값 중 하나입니다.</returns>
        public static MessageBoxWindowResult Show(String text, String caption)
        {
            return Show(new MessageBoxViewModel()
            {
                Caption = caption,
                Header = text
            });
        }

        /// <summary>
        /// 지정된 텍스트와 캡션이 있는 메시지 상자를 표시합니다.
        /// </summary>
        /// <param name="text">메시지 상자에 표시할 텍스트입니다.</param>
        /// <param name="caption">메시지 상자의 제목 표시줄에 표시할 텍스트입니다.</param>
        /// <param name="buttons">메시지 상자에 표시할 단추를 지정하는 Yuhan.WPF.MessageBox.MessageBoxWindowButtons 값 중 하나입니다.</param>
        /// <returns>Yuhan.WPF.MessageBox.MessageBoxWindowResult 값 중 하나입니다.</returns>
        public static MessageBoxWindowResult Show(String text, String caption, MessageBoxWindowButtons buttons)
        {
            return Show(
                new MessageBoxViewModel()
                {
                    Caption = caption,
                    Header = text,
                    Buttons = buttons
                });
        }

        /// <summary>
        /// 지정된 텍스트와 캡션, 하단 텍스트가 있는 메시지 상자를 표시합니다.
        /// </summary>
        /// <param name="text">메시지 상자에 표시할 텍스트입니다.</param>
        /// <param name="caption">메시지 상자의 제목 표시줄에 표시할 텍스트입니다.</param>
        /// <param name="buttons">메시지 상자에 표시할 단추를 지정하는 Yuhan.WPF.MessageBox.MessageBoxWindowButtons 값 중 하나입니다.</param>
        /// <param name="footerText">메시지 상자의 하단 표시줄에 표시할 텍스트입니다.</param>
        /// <returns>Yuhan.WPF.MessageBox.MessageBoxWindowResult 값 중 하나입니다.</returns>
        public static MessageBoxWindowResult Show(String text, String caption, String footerText, MessageBoxWindowButtons buttons)
        {
            return Show(
                new MessageBoxViewModel()
                {
                    Caption = caption,
                    Header = text,
                    FooterText = footerText,
                    Buttons = buttons
                });
        }

        /// <summary>
        //     지정된 텍스트, 캡션, 단추 및 아이콘이 있는 메시지 상자를 표시합니다.
        /// </summary>
        /// <param name="text">메시지 상자에 표시할 텍스트입니다.</param>
        /// <param name="caption">메시지 상자의 제목 표시줄에 표시할 텍스트입니다.</param>
        /// <param name="buttons">메시지 상자에 표시할 단추를 지정하는 Yuhan.WPF.MessageBox.MessageBoxWindowButtons 값 중 하나입니다.</param>
        /// <param name="icon">메시지 상자에 표시할 아이콘을 지정하는 Yuhan.WPF.MessageBox.MessageBoxWindowIcons 값 중 하나입니다.</param>
        /// <returns>Yuhan.WPF.MessageBox.MessageBoxWindowResult 값 중 하나입니다.</returns>
        public static MessageBoxWindowResult Show(String text, String caption, MessageBoxWindowButtons buttons, MessageBoxWindowIcons icon)
        {
            return Show(
                new MessageBoxViewModel()
                {
                    Caption = caption,
                    HeaderIcon = icon,
                    Header = text,
                    Buttons = buttons
                });
        }

        /// <summary>
        //     지정된 텍스트, 설명, 캡션, 단추 및 아이콘이 있는 메시지 상자를 표시합니다.
        /// </summary>
        /// <param name="text">메시지 상자에 표시할 텍스트입니다.</param>
        /// <param name="caption">메시지 상자의 제목 표시줄에 표시할 텍스트입니다.</param>
        /// <param name="subscription">메시지 상자의 표시 텍스트 하단의 설명 텍스트입니다.</param>
        /// <param name="buttons">메시지 상자에 표시할 단추를 지정하는 Yuhan.WPF.MessageBox.MessageBoxWindowButtons 값 중 하나입니다.</param>
        /// <param name="icon">메시지 상자에 표시할 아이콘을 지정하는 Yuhan.WPF.MessageBox.MessageBoxWindowIcons 값 중 하나입니다.</param>
        /// <returns>Yuhan.WPF.MessageBox.MessageBoxWindowResult 값 중 하나입니다.</returns>
        public static MessageBoxWindowResult Show(String text, String caption, String subscription, MessageBoxWindowButtons buttons, MessageBoxWindowIcons icon)
        {
            return Show(
                new MessageBoxViewModel()
                {
                    Caption = caption,
                    HeaderIcon = icon,
                    Header = text,
                    Description = subscription,
                    Buttons = buttons
                });
        }

        /// <summary>
        //     지정된 텍스트, 설명, 부가정보 및 캡션, 단추 및 아이콘이 있는 메시지 상자를 표시합니다.
        /// </summary>
        /// <param name="text">메시지 상자에 표시할 텍스트입니다.</param>
        /// <param name="caption">메시지 상자의 제목 표시줄에 표시할 텍스트입니다.</param>
        /// <param name="subscription">메시지 상자의 표시 텍스트 하단의 설명 텍스트입니다.</param>
        /// <param name="details">메시지 상자의 숨겨진 부가 정보 설명의 텍스트입니다.</param>
        /// <param name="buttons">메시지 상자에 표시할 단추를 지정하는 Yuhan.WPF.MessageBox.MessageBoxWindowButtons 값 중 하나입니다.</param>
        /// <param name="icon">메시지 상자에 표시할 아이콘을 지정하는 Yuhan.WPF.MessageBox.MessageBoxWindowIcons 값 중 하나입니다.</param>
        /// <returns>Yuhan.WPF.MessageBox.MessageBoxWindowResult 값 중 하나입니다.</returns>
        public static MessageBoxWindowResult Show(String text, String caption, String subscription, String details, MessageBoxWindowButtons buttons, MessageBoxWindowIcons icon)
        {
            return Show(
                new MessageBoxViewModel()
                {
                    Caption = caption,
                    HeaderIcon = icon,
                    Header = text,
                    Description = subscription,
                    Details = details,
                    Buttons = buttons
                });
        }

        /// <summary>
        /// ViewModel을 사용한 메세지 상자를 표시합니다.
        /// </summary>
        /// <param name="viewModel">Yuhan.WPF.MessageBox.MessageBoxViewModel</param>
        /// <returns>Yuhan.WPF.MessageBox.MessageBoxWindowResult 값 중 하나입니다.</returns>
        public static MessageBoxWindowResult Show(MessageBoxViewModel viewModel)
        {
            return MessageBoxWindow.Show(viewModel);
        }
    }
}
