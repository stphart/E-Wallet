using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BongancisoE_wallet
{
    public partial class WebForm4 : System.Web.UI.Page
    {
        String connDB = WebConfigurationManager.ConnectionStrings["conndb"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                
                String username = Session["cp_username"].ToString();
                if (username == "")
                {
                    panel1();
                }
                else
                {
                    panel2();
                }
            }

        }

        void panel1()
        {
            error_search.Visible = false;
            search_user.Visible = true;
            display_user.Visible = false;
        }

        void panel2()
        {
            display_user.Visible = true;
            search_user.Visible = false;
            display_user_acc();
        }


        void display_user_acc()
        {
            try { 
            String username = Session["cp_username"].ToString();
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "SELECT lname, fname, email FROM usersAcc WHERE username ='" + username + "' ";
                    SqlDataReader reader = query.ExecuteReader();

                    if (reader.Read())
                    {

                        user_lname.Text = reader["lname"].ToString();
                        user_fname.Text = reader["fname"].ToString();
                        
                    }
                   
                }
                db.Close();
            }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error!')</script>");
            }
        }

        protected void btn_cancel_Click(object sender, EventArgs e)
        {
            findAcc.Text = "";
            Response.Redirect("BongancisoLogin");
        }

        protected void btn_search_Click(object sender, EventArgs e)
        {
            try { 
            var username = findAcc.Text;
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "SELECT username FROM usersAcc WHERE username ='" + username + "' OR " +
                        "email ='" + username + "' OR " +
                        "mobilenum ='" + username + "' ";
                    SqlDataReader reader = query.ExecuteReader();

                    if (reader.Read())
                    {
                        Session["cp_username"] = reader["username"].ToString();
                        
                        panel2();
                    }
                    else
                    {
                        error_search.Visible = true;
                    }

                }
                db.Close();
            }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error!')</script>");
            }
        }

        protected void btn_not_acc_Click(object sender, EventArgs e)
        {
            Session["cp_username"] = "";
            panel1();
        }

        protected void btn_confirm_acc_Click(object sender, EventArgs e)
        {
            try { 
            String username = Session["cp_username"].ToString();
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "SELECT email, lname, fname, accnumber FROM usersAcc WHERE username ='" + username + "' ";
                    SqlDataReader reader = query.ExecuteReader();

                    if (reader.Read())
                    {

                        Session["cp_email"] = reader["email"].ToString();
                        Session["cp_accnum"] = reader["accnumber"].ToString();
                        Session["cp_name"] = reader["fname"].ToString() + " " + reader["lname"].ToString();
                        Response.Redirect("BongancisoChangePassword");

                    }

                }
                db.Close();
            }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error!')</script>");
            }
        }
    }
}