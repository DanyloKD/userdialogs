﻿using System;
using UIKit;


namespace Acr.UserDialogs
{

    public class ProgressDialog : IProgressDialog
    {
        readonly ProgressDialogConfig config;


        public ProgressDialog(ProgressDialogConfig config)
        {
            this.config = config;
            this.title = config.Title;
        }


        #region IProgressDialog Members

        string title;
        public virtual string Title
        {
            get { return this.title; }
            set
            {
                if (this.title == value)
                    return;

                this.title = value;
                this.Refresh();
            }
        }


        int percentComplete;
        public virtual int PercentComplete
        {
            get { return this.percentComplete; }
            set
            {
                if (this.percentComplete == value)
                    return;

                if (value > 100)
                    this.percentComplete = 100;
                else if (value < 0)
                    this.percentComplete = 0;
                else
                    this.percentComplete = value;
                this.Refresh();
            }
        }


        public virtual bool IsShowing { get; private set; }


        public virtual void Show()
        {
            this.IsShowing = true;
            this.Refresh();
        }


        public virtual void Hide()
        {
            this.IsShowing = false;
#if __IOS__
            UIApplication.SharedApplication.InvokeOnMainThread(BTProgressHUD.BTProgressHUD.Dismiss);
#endif
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            this.Hide();
        }

        #endregion

        #region Internals

        protected virtual void Refresh()
        {
            if (!this.IsShowing)
                return;

            var txt = this.Title;
            float p = -1;
            if (this.config.IsDeterministic)
            {
                p = (float)this.PercentComplete / 100;
                if (!String.IsNullOrWhiteSpace(txt))
                {
                    txt += "... ";
                }
                txt += this.PercentComplete + "%";
            }

            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                if (this.config.OnCancel == null)
                {
#if __IOS__
                    BTProgressHUD.BTProgressHUD.Show(
                        this.Title,
                        p,
                        this.config.MaskType.ToNative()
                    );
#endif
                }
                else
                {
#if __IOS__
                    BTProgressHUD.BTProgressHUD.Show(
                        this.config.CancelText,
                        this.config.OnCancel,
                        txt,
                        p,
                        this.config.MaskType.ToNative()
                    );
#endif
                }
            });
        }

#endregion
    }
}
