using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net;

namespace BongancisoE_wallet
{
    public partial class BongancisoReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                displayerror.Visible = false;
               
            }
        }

        protected void btn_user1_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoDeposit");
        }
        protected void btn_user2_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoWithdraw");
        }

        protected void btn_user3_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoSend");
        }

        protected void btn_user4_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoSOA");
        }

        protected void btn_send_Click(object sender, EventArgs e)
        {
            try { 
            //send email to user
            //send email to ewallet
            if(drp1.SelectedValue == "Others" && othermsg.Text == "" || txt1.Text == "" || txt2.Text == "" || msginput.Text == "")
            {
                displayerror.Visible=true;
                displayerror.Text = "Kindly fill up everything!";
                displayerror.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                send_email_to_wallet();
                send_email_to_user();
                displayerror.Visible=true;
                displayerror.Text = "Your inquiry has been sent. Kindly wait for the feedback, our team will be working on it. Thank you!";
                displayerror.ForeColor = System.Drawing.Color.Green;
                txt1.Text = "";
                txt2.Text = "";
                msginput.Text = "";
                drp1.SelectedValue = "Transaction Delays";
                othermsg.Text = "";

            }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error!')</script>");
            }


        }

        void send_email_to_user()
        {
            var email = txt2.Text; ;
            var name = txt1.Text;
            String topic = "";
            if (drp1.SelectedValue == "Others")
            {
                topic = othermsg.Text;
            }
            else
            {
                topic = drp1.SelectedValue;
            }

            var bodymsg = "Hi "+ name  +
                ", <br>    Thanks for contacting E-Wallet | B <br>" +
                "   This is an automatic reply to let you notice that we receive your inquiry about " + topic +
                ". We will get back to you with a response as quickly as possible. We hope for your understanding and patience. <br>" +
                "   If you have any additional information that you think will help us assits you, feel free to reach out to this email. <br> <br>" +

                "Grateful, <br>" + "E-Wallet | B";


            MailMessage msg = new MailMessage("E-Wallet | Bonganciso <ewallet.shb@gmail.com>", email);
            msg.Subject = "Thank you for reaching out E-Wallet | B";
            msg.Body = bodymsg;
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.High;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential()
            {
                UserName = "ewallet.shb@gmail.com",
                Password = "epnakkmvtjnghpwq"
            };
            smtp.EnableSsl = true;
            smtp.Send(msg);
            

        }

        void send_email_to_wallet()
        {
            var email = txt2.Text; ;
            var name = txt1.Text;
            String topic = "";
            if (drp1.SelectedValue == "Others")
            {
                topic = othermsg.Text;
            }
            else
            {
                topic = drp1.SelectedValue;
            }
            var msg = msginput.Text;
            var bodymsg = "Name : " +name +
                "<br>Email : " + email +
                "<br>Inquiry : " + topic +
                "<br>Message : " + msg;


            MailMessage msg2 = new MailMessage("E-Wallet | Bonganciso <ewallet.shb@gmail.com>", "ewallet.shb@gmail.com");
            msg2.Subject = "Contact Us (" +topic+ ")";
            msg2.Body = bodymsg;
            msg2.IsBodyHtml = true;
            msg2.Priority = MailPriority.High;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential()
            {
                UserName = "ewallet.shb@gmail.com",
                Password = "epnakkmvtjnghpwq"
            };
            smtp.EnableSsl = true;
            smtp.Send(msg2);


        }

        protected void btn_link1_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoDashboard");
        }
        protected void btn_link2_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoUser");
        }
    }
}