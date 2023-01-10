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
    
    public partial class WebForm2 : System.Web.UI.Page
    {
        String connDB = WebConfigurationManager.ConnectionStrings["conndb"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                msg.Visible = false;
            }
            
        }

        protected void btnlogin_Click(object sender, EventArgs e)
        {
            try { 
            String getUsername = txtusername.Text;
            String getPassword = txtpassword.Text;

            //read database
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using(var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "SELECT username, fname, lname, accnumber, datecreated, email FROM usersAcc WHERE username ='"+ getUsername+"' OR email = '" + getUsername+"' " +
                        " AND password='"+getPassword+"' ";
                    SqlDataReader reader = query.ExecuteReader();

                    if(reader.Read())
                    {
                        
                        Session["username"] = reader["username"].ToString();
                        Session["name"] = reader["fname"].ToString() + " " + reader["lname"].ToString();
                        Session["accnum"] = reader["accnumber"].ToString();
                        Session["date"] = reader["datecreated"].ToString();
                        Session["email"] = reader["email"].ToString();
                        Response.Redirect("BongancisoDashboard");
                    }
                    else
                    {
                        msg.Visible = true;
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

        protected void forgotPassword_Click(object sender, EventArgs e)
        {
            Session["cp_username"] = txtusername.Text;
            Response.Redirect("BongancisoForgotPassword");
        }
    }
}