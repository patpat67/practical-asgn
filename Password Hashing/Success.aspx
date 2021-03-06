<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Success.aspx.cs" Inherits="Password_Hashing.Success" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Red+Hat+Display:wght@300;400;500;600;700;800;900&display=swap');

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        .main-container {
            min-width: 100vw;
            min-height: 100vh;
            display: flex;
            align-content: center;
            justify-content: center;
            font-family: 'Red Hat Display', sans-serif;
            background: url('../images/pattern-background-desktop.svg')no-repeat top;
            background-color: #E0E8FE;
        }

        .container {
            width: 450px;
            min-height: 550px;
            margin: auto;
            border-radius: 16px;
        }

        .top-part {
            height: 221px;
            background: url('../images/illustration-hero.svg') no-repeat center;
            border-top-left-radius: 16px;
            border-top-right-radius: 16px;
        }

        .bottom-part {
            background-color: #fff;
            border-bottom-left-radius: 16px;
            border-bottom-right-radius: 16px;
        }

        .word-section {
            text-align: center;
            padding-top: 22px;
        }

            .word-section h1 {
                font-size: 28px;
                padding: 12px;
                color: hsl(223, 47%, 23%);
                font-weight: 700;
            }

            .word-section p {
                font-size: 17px;
                letter-spacing: 0.4px;
                line-height: 23px;
                font-weight: 600;
                color: hsl(226, 20%, 71%);
                padding: 12px;
                margin-bottom: 15px;
            }

        .plan-section {
            background-color: hsl(225, 100%, 98%);
            display: flex;
            flex-direction: row;
            padding: 16px;
            border-radius: 12px;
            align-items: center;
            margin: 0 40px;
            margin-bottom: 32px;
        }

            .plan-section .img img {
                margin-right: 15px;
            }

            .plan-section .annual-plan h2 {
                color: hsl(223, 47%, 23%);
                font-size: 20px;
            }

            .plan-section .annual-plan p {
                color: hsl(226, 20%, 71%);
                font-weight: 600;
            }

            .plan-section .change {
                margin: auto;
                margin-right: 0px;
            }

                .plan-section .change a {
                    color: hsl(245, 75%, 52%);
                    text-decoration: underline;
                    font-weight: 600;
                    transition: 0.3s ease-out;
                    transition-property: color, text;
                }

                    .plan-section .change a:hover {
                        color: #766CF1;
                        text-decoration: none;
                    }

        .btn-section {
            display: flex;
            flex-direction: column;
            margin-top: 8px;
            padding-bottom: 32px;
        }

        .btn {
            border: none;
            width: auto;
            margin: 0 40px;
            font-size: 14px;
            padding: 14px 0;
            border-radius: 7px;
            font-family: 'Red Hat Display', sans-serif;
            font-weight: 700;
            cursor: pointer;
        }

        .btn-payment {
            background-color: hsl(245, 75%, 52%);
            margin-bottom: 25px;
            color: #fff;
            transition: 0.3s ease-in;
            transition-property: background;
            box-shadow: 0 18px 14px 0px rgba(0, 0, 0, 0.2);
        }

            .btn-payment:hover {
                background-color: #766CF1;
            }

        .btn-submit {
            color: hsl(226, 20%, 71%);
            background-color: #fff;
            transition: 0.3s ease-in;
            transition-property: color;
        }

            .btn-submit:hover {
                color: hsl(223, 47%, 23%);
            }

        /* Media Queries */

        @media (min-width: 1440px) {
            .main-container {
                background-image: none;
            }
        }

        @media (max-height: 700px) {
            .main-container {
                padding: 35px 0;
            }
        }

        @media (max-width: 500px) {
            .main-container {
                padding: 35px 35px;
            }

            .plan-section .annual-plan h2 {
                font-size: 16px;
            }

            .plan-section .annual-plan p {
                font-size: 14px;
            }

            .plan-section .change a {
                font-size: 14px;
            }
        }

        @media (max-width: 375px) {
            .main-container {
                background-image: url('../images/pattern-background-mobile.svg');
                padding: 25px 25px;
            }

            .container {
                max-width: 350px !important;
            }

            br {
                display: none;
            }

            .word-section p {
                margin: 0 18px;
            }

            .plan-section {
                margin: 0 20px;
            }

            .annual-plan {
                margin-right: 15px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <%-- end of reference --%>
                  <div class="main-container">
        <div class="container">
            <div class="top-part"></div>
            <div class="bottom-part">
                <div class="word-section">
                    <h1>Login Success!</h1>
                    <p>Welcome to SITConnect!</p>
                </div>
<%--                <div class="plan-section">
                    <div class="img">
                        <img src="./images/icon-music.svg" alt="">
                    </div>
                    <div class="annual-plan">
                        <h2>Annual Plan</h2>
                        <p>$59.99/year</p>
                    </div>
                    <div class="change">
                        <a href="#">Change</a>
                    </div>
                </div>--%>
                <div class="plan-section">
<%--                    <label class="inputLabel">Notification:</label>--%>
                    <asp:Label ID="lbl_Message" runat="server" ForeColor="Red"></asp:Label>

                </div>
                <div class="plan-section">
                    <label class="inputLabel">Email:</label>
                    &nbsp&nbsp&nbsp
                    <asp:Label ID="lbl_email" runat="server" ForeColor="Green"></asp:Label>

                </div>
                <div class="plan-section">
                    <label class="inputLabel">First Name:</label>
                    &nbsp&nbsp&nbsp
                    <asp:Label ID="lbl_fname" runat="server" ForeColor="Green"></asp:Label>

                </div>
                <div class="plan-section">
                    <label class="inputLabel">Last Name:</label>
                    &nbsp&nbsp&nbsp
                    <asp:Label ID="lbl_lname" runat="server" ForeColor="Green"></asp:Label>

                </div>
                <div class="plan-section">
                    <label class="inputLabel">Credit Card Information:</label>
                    &nbsp&nbsp&nbsp
                    <asp:Label ID="lbl_credit" runat="server" ForeColor="Green"></asp:Label>

                </div>
                <div class="btn-section">
                                    <asp:Button ID="ChangePassBtn" runat="server" Text="Change Password" OnClick="ChangeOfPassword" class="btn-payment btn" />
                                    <asp:Button ID="LogoutBtn" runat="server" Text="Log Out" OnClick="LogoutOfAccount" class="btn-payment btn" />
                </div>
            </div>
        </div>
        </div>
        <%-- delete portion aka reference portion --%>
    </form>
</body>
</html>
