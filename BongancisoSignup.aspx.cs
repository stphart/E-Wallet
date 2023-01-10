using System;
using System.Collections.Generic; 
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace BongancisoE_wallet
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        String conndb = WebConfigurationManager.ConnectionStrings["conndb"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblusername.Visible = false;
                lblemail.Visible = false;
                signupPanel.Visible = true;
                panelVer.Visible = false;
                incorrect_code.Visible = false;
                confirm_panel.Visible = false;
            }
            
        }

        protected void btnsignup_Click(object sender, EventArgs e)
        {
            try { 
            //get the value
            
            String getusername = usernametxt.Text;
            
            //check if username is unique
            //if the username already exists
            //or already register to as one of the accounts
            //user will have to re-enter their username
            bool check = checkUsername(getusername);
            bool checkEm = checkEmail(emailaddtxt.Text);

            if (check == true)
            {
                lblusername.Visible = true;
                usernametxt.Text = "";
            }
            if(checkEm == true)
                {
                    lblemail.Visible = true;
                    emailaddtxt.Text = "";
                }
            else
            {
                Session["signup1"] = lastnametxt.Text;
                Session["signup2"] = firstnametxt.Text;
                Session["signup3"] = mobilenumtxt.Text;
                Session["signup4"] = emailaddtxt.Text;
                Session["signup5"] = usernametxt.Text;
                Session["signup6"] = password1.Text;
                Session["signup7"] = datetxt.Text;
                Session["signup8"] = gendertext.SelectedValue;
                        

                display_email.Text = emailaddtxt.Text;
                lblusername.Visible = false;
                lblemail.Visible=false;
                signupPanel.Visible = false;
                send_email();
                panelVer.Visible = true;
                incorrect_code.Visible = false;

            }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error!')</script>");
            }
        }


        Boolean checkUsername(String username)
        {
            //this function is to check the username of the user
            //if it already exists on the list
            //if not the user can proceed on signing up
            using (var db = new SqlConnection(conndb))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "SELECT * FROM usersAcc WHERE username ='" + username + "' ";
                    SqlDataReader reader = query.ExecuteReader();

                    if (reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }

            }
        }

        Boolean checkEmail(String email)
        {
            //this function is to check the username of the user
            //if it already exists on the list
            //if not the user can proceed on signing up
            using (var db = new SqlConnection(conndb))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "SELECT * FROM usersAcc WHERE email ='" + email + "' ";
                    SqlDataReader reader = query.ExecuteReader();

                    if (reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }

            }
        }

        long generate_number()
        {
            Random random = new Random();
            long number = random.Next(100000000, 999999999);
            return number;
        }

        void check_accnum()
        {
            long number = generate_number();
            using (var db = new SqlConnection(conndb))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "SELECT * FROM usersAcc WHERE accnumber ='" + number + "' ";
                    SqlDataReader reader = query.ExecuteReader();

                    while (reader.Read())
                    {
                        check_accnum();
                    }

                    Session["accnum"] = number;


                }

            }
        }

        protected void btn_alreadyhaveacc_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoLogin");
        }

        protected void btn_signup_Click(object sender, EventArgs e)
        {
            //we have to verify first if the code inputted by user
            //is the same code as the one the system send thru the email

            var inputcode = Convert.ToInt32(txtverify.Text);
            var code = Convert.ToInt32(Session["generate_num"]);

            if (inputcode != code)
            {
                incorrect_code.Visible = true;
            }
            else
            {
                txtverify.Text = "";
                createAcc();
            }

        }

        void createAcc()
        {
            try { 
                //get the value
                String getlname = Session["signup1"].ToString();
                String getfname = Session["signup2"].ToString();
                String getnum = Session["signup3"].ToString();
                String getemail = Session["signup4"].ToString();
                String getusername = Session["signup5"].ToString();
                String getpassword = Session["signup6"].ToString();
                    String getbirth = Session["signup7"].ToString();
                    String getgender = Session["signup8"].ToString();

                check_accnum();
                String getAccnum = Session["accnum"].ToString();
                //code to write in the database
                //the user's information will then be saved to the database
                //also an initinal 2000 balance will be added as a new account has been made
                using (var db = new SqlConnection(conndb))
                {
                    db.Open();
                    using (var query = db.CreateCommand())
                    {
                        query.CommandType = CommandType.Text;
                        query.CommandText = "INSERT INTO usersAcc (lname, fname, accnumber, mobilenum, email, username, password, datecreated, balance, datebirth, gender) "
                                             + " VALUES (@lname, @fname, @accnum, @num, @email, @username, @password, @date, @balance, @birth, @gender)";

                        query.Parameters.AddWithValue("@lname", getlname);
                        query.Parameters.AddWithValue("@fname", getfname);
                        query.Parameters.AddWithValue("@accnum", getAccnum);
                        query.Parameters.AddWithValue("@num", getnum);
                        query.Parameters.AddWithValue("@email", getemail);
                        query.Parameters.AddWithValue("@username", getusername);
                        query.Parameters.AddWithValue("@password", getpassword);
                        query.Parameters.AddWithValue("@date", DateTime.Now);
                        query.Parameters.AddWithValue("@balance", 2000.00);
                        query.Parameters.AddWithValue("@birth", getbirth);
                        query.Parameters.AddWithValue("@gender", getgender);

                            var ctr = query.ExecuteNonQuery();

                        if (ctr > 0)
                        {
                            panelVer.Visible = false;
                            signupPanel.Visible = false;
                            confirm_panel.Visible = true;
                        }
                        else
                        {
                            Response.Write("<script>alert('Error! here')</script>");
                        }
                    }
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error! lol')</script>");
            }
        }

        int generate_code()
        {
            Random random = new Random();
            int number = random.Next(10000, 99999);
            Session["generate_num"] = number;
            return number;

        }

        void send_email()
        {
            int num = generate_code();
            var email = Session["signup4"].ToString();
            var username = Session["signup5"].ToString();
            var name = Session["signup2"].ToString() + " " + Session["signup1"].ToString();
            var bodymsg = "Hi " + name + "! <br>Confirm your email address to complete your account's registration. <br>" +
                "<br><br>To confirm this action, here's your verification code: " + "<h2>" + num + "</h2> <br><br>" +
                "If you did not make this request then kindly ignore this email. <br><br>" +
                "Thanks, <br>" + "E-Wallet | B";


            MailMessage msg = new MailMessage("E-Wallet | Bonganciso <ewallet.shb@gmail.com>", email);
            msg.Subject = "Verify Account";
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

        protected void btn_nocode_Click(object sender, EventArgs e)
        {
            send_email();
        }

        protected void btn_addnew_Click(object sender, EventArgs e)
        {
            panelVer.Visible = false;
            confirm_panel.Visible = false;
            Session.Abandon();
            signupPanel.Visible = true;
        }

        protected void btn_continue_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoLogin");
        }

    }
}