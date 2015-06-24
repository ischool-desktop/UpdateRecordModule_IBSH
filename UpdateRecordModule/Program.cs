using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Permission;
using FISCA.Presentation;
using FISCA;
using FISCA.Deployment;
using K12.Data;
using Campus.DocumentValidator;
using SmartSchool.Customization.Data;

namespace UpdateRecordModule_IBSH
{
    /// <summary>
    /// 程式被載入點(高中學籍日校)
    /// </summary>
    public class Program
    {
        public const string UpdateRecordGovDocsCode = "Button0630";
        public const string UpdateRecordContentCode = "Content0140";
        public const string BeforeEnrollmentContentCode = "SHSchool.Student.BeforeEnrollmentItem";
        /// <summary>
        /// 高中學籍日校系統核心啟動
        /// </summary>        
        [FISCA.MainMethod("UpdateRecordModule_SH_D")]
        public static void Main()
        {
            #region 自訂驗證規則
            FactoryProvider.RowFactory.Add(new ValidationRule.CounselRowValidatorFactory());
            #endregion

            // 異動
            if (FISCA.Permission.UserAcl.Current[UpdateRecordContentCode].Editable || FISCA.Permission.UserAcl.Current[UpdateRecordContentCode].Viewable)
                K12.Presentation.NLDPanels.Student.AddDetailBulider(new FISCA.Presentation.DetailBulider<UpdateRecordItem>());

            // 前籍畢業資訊
            if (FISCA.Permission.UserAcl.Current[BeforeEnrollmentContentCode].Editable || FISCA.Permission.UserAcl.Current[BeforeEnrollmentContentCode].Viewable)
                K12.Presentation.NLDPanels.Student.AddDetailBulider(new FISCA.Presentation.DetailBulider<BeforeEnrollmentItem>());

            Catalog detail = RoleAclSource.Instance["學生"]["資料項目"];
            detail.Add(new DetailItemFeature(typeof(UpdateRecordItem)));
            detail.Add(new DetailItemFeature(typeof(BeforeEnrollmentItem)));
            Catalog ribbon = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            ribbon.Add(new RibbonFeature(UpdateRecordGovDocsCode, "函報名冊"));
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SmartSchool.Others.RibbonBars.NameList));
            var btnItemNameList = MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["異動作業"];
            btnItemNameList.Image = Properties.Resources.history_save_64;
            btnItemNameList.Size = RibbonBarButton.MenuButtonSize.Large;
            btnItemNameList["函報名冊"].Enable = FISCA.Permission.UserAcl.Current[UpdateRecordGovDocsCode].Executable;
            btnItemNameList["函報名冊"].Click += delegate
            {
                new GovernmentalDocument.ListForm().ShowDialog();
            };

            // 匯出新生異動
            RibbonBarButton rbEnrollmentListExport = MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"];
            rbEnrollmentListExport["異動相關匯出"]["匯出新生異動"].Enable = FISCA.Permission.UserAcl.Current["Button0200"].Executable;
            rbEnrollmentListExport["異動相關匯出"]["匯出新生異動"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new UpdateRecordModule_IBSH.ImportExport.ExportEnrollmentList();
                ImportExport.ExportStudentV2 wizard = new ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            // 匯入新生異動
            RibbonBarButton rbEnrollmentListImport = MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"];
            rbEnrollmentListImport["異動相關匯入"]["匯入新生異動"].Enable = FISCA.Permission.UserAcl.Current["Button0280"].Executable;
            rbEnrollmentListImport["異動相關匯入"]["匯入新生異動"].Click += delegate
            {
                Global._AllStudentNumberStatusIDTemp = DAL.FDQuery.GetAllStudenNumberStatusDict();
                Global._StudentUpdateRecordTemp = DAL.FDQuery.GetHasStudentUpdateRecord01Dict(1, 99);
                ImportExport.ImportEnrollmentList iel = new ImportExport.ImportEnrollmentList();
                iel.Execute();

            };

            // 匯出轉入異動
            RibbonBarButton rbTransferListExport = MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"];
            rbTransferListExport["異動相關匯出"]["匯出轉入異動"].Enable = FISCA.Permission.UserAcl.Current["Button0200"].Executable;
            rbTransferListExport["異動相關匯出"]["匯出轉入異動"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new UpdateRecordModule_IBSH.ImportExport.ExportTransferList();
                ImportExport.ExportStudentV2 wizard = new ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            // 匯入轉入異動
            RibbonBarButton rbTransferListImport = MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"];
            rbTransferListImport["異動相關匯入"]["匯入轉入異動"].Enable = FISCA.Permission.UserAcl.Current["Button0280"].Executable;
            rbTransferListImport["異動相關匯入"]["匯入轉入異動"].Click += delegate
            {
                Global._AllStudentNumberStatusIDTemp = DAL.FDQuery.GetAllStudenNumberStatusDict();
                Global._StudentUpdateRecordTemp = DAL.FDQuery.GetHasStudentUpdateRecord01Dict(100, 199);
                ImportExport.ImportTransferList iel = new ImportExport.ImportTransferList();
                iel.Execute();

            };

            // 匯出學籍異動
            RibbonBarButton rbStudUpdateRecordExport = MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"];
            rbStudUpdateRecordExport["異動相關匯出"]["匯出學籍異動"].Enable = FISCA.Permission.UserAcl.Current["Button0200"].Executable;
            rbStudUpdateRecordExport["異動相關匯出"]["匯出學籍異動"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new UpdateRecordModule_IBSH.ImportExport.ExportStudUpdateRecordList();
                ImportExport.ExportStudentV2 wizard = new ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            // 匯入學籍異動
            RibbonBarButton rbStudUpdateRecordImport = MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"];
            rbStudUpdateRecordImport["異動相關匯入"]["匯入學籍異動"].Enable = FISCA.Permission.UserAcl.Current["Button0280"].Executable;
            rbStudUpdateRecordImport["異動相關匯入"]["匯入學籍異動"].Click += delegate
            {
                Global._AllStudentNumberStatusIDTemp = DAL.FDQuery.GetAllStudenNumberStatusDict();
                Global._StudentUpdateRecordTemp = DAL.FDQuery.GetHasStudentUpdateRecord01Dict(200, 500);
                ImportExport.ImportStudUpdateRecordList iel = new ImportExport.ImportStudUpdateRecordList();
                iel.Execute();

            };

            // 匯出畢業異動
            RibbonBarButton rbGraduateExport = MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"];
            rbGraduateExport["異動相關匯出"]["匯出畢業異動"].Enable = FISCA.Permission.UserAcl.Current["Button0200"].Executable;
            rbGraduateExport["異動相關匯出"]["匯出畢業異動"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new UpdateRecordModule_IBSH.ImportExport.ExportGraduateList();
                ImportExport.ExportStudentV2 wizard = new ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            // 匯入畢業異動
            RibbonBarButton rbGraduateImport = MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"];
            rbGraduateImport["異動相關匯入"]["匯入畢業異動"].Enable = FISCA.Permission.UserAcl.Current["Button0280"].Executable;
            rbGraduateImport["異動相關匯入"]["匯入畢業異動"].Click += delegate
            {
                Global._AllStudentNumberStatusIDTemp = DAL.FDQuery.GetAllStudenNumberStatusDict();
                Global._StudentUpdateRecordTemp = DAL.FDQuery.GetHasStudentUpdateRecord01Dict(500, 502);
                ImportExport.ImportGraduateList iel = new ImportExport.ImportGraduateList();
                iel.Execute();

            };

            Catalog ribbon1 = RoleAclSource.Instance["學生"]["功能按鈕"];
            ribbon1.Add(new RibbonFeature("SHSchool.Student.RibbonImportUpdateRecCode100", "匯入學籍異動"));
            ribbon1.Add(new RibbonFeature("Button0200", "匯出異動相關"));
            ribbon1.Add(new RibbonFeature("Button0280", "匯入異動相關"));

            // 批次新生異動
            RibbonBarButton rbBatchNewUpdateRec = MotherForm.RibbonBarItems["學生", "教務"]["新生作業"];
            rbBatchNewUpdateRec["批次新生異動"].Enable = FISCA.Permission.UserAcl.Current["SHSchool.Student.rbBatchNewUpdateRec001"].Executable;
            rbBatchNewUpdateRec["批次新生異動"].Click += delegate
            {
                Batch.BatchNewStudUpdateRecForm bnsur = new Batch.BatchNewStudUpdateRecForm();
                bnsur.ShowDialog();
            };
            ribbon1.Add(new RibbonFeature("SHSchool.Student.rbBatchNewUpdateRec001", "批次新生異動"));

            Catalog ribbon2 = RoleAclSource.Instance["學生"]["功能按鈕"];

            // 批次畢業異動
            RibbonBarButton rbBatchGraduateRec = MotherForm.RibbonBarItems["學生", "教務"]["畢業作業"];
            rbBatchGraduateRec["批次畢業異動"].Enable = FISCA.Permission.UserAcl.Current["SHSchool.Student.rbBatchGraduateRec001"].Executable;
            rbBatchGraduateRec["批次畢業異動"].Click += delegate
            {
                Batch.BatchGraduateRecForm bgrf = new Batch.BatchGraduateRecForm();
                bgrf.ShowDialog();
            };
            ribbon2.Add(new RibbonFeature("SHSchool.Student.rbBatchGraduateRec001", "批次畢業異動"));

            // 加入權限代碼
            ribbon2.Add(new RibbonFeature("SHSchool.Student.UpdateRecordForm.Button01", "轉科"));
            ribbon2.Add(new RibbonFeature("SHSchool.Student.UpdateRecordForm.Button02", "學籍更正"));

            var btnStudentUR = K12.Presentation.NLDPanels.Student.RibbonBarItems["教務"]["異動作業"];
            btnStudentUR.Image = Properties.Resources.demographic_reload_64;
            btnStudentUR.Size = RibbonBarButton.MenuButtonSize.Large;
            //var btnChangeDept = btnStudentUR["轉科"];
            //btnChangeDept.Click += new EventHandler(btnChangeDept_Click);
            //btnChangeDept.Enable = K12.Presentation.NLDPanels.Student.SelectedSource.Count == 1;
            var btnChangeInfo = btnStudentUR["學籍更正"];
            btnChangeInfo.Click += new EventHandler(btnChangeInfo_Click);
            btnChangeInfo.Enable = K12.Presentation.NLDPanels.Student.SelectedSource.Count == 1;

            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                //btnChangeDept.Enable = K12.Presentation.NLDPanels.Student.SelectedSource.Count == 1 && FISCA.Permission.UserAcl.Current["SHSchool.Student.UpdateRecordForm.Button01"].Executable;
                btnChangeInfo.Enable = K12.Presentation.NLDPanels.Student.SelectedSource.Count == 1 && FISCA.Permission.UserAcl.Current["SHSchool.Student.UpdateRecordForm.Button02"].Executable;
            };

            // 加入權限代碼
            ribbon2.Add(new RibbonFeature("SHSchool.Student.DeleteUpdateRecordForm", "刪除異動資料"));

            // 刪除異動資料            
            K12.Presentation.NLDPanels.Student.ListPaneContexMenu["刪除異動資料"].Enable = UserAcl.Current["SHSchool.Student.DeleteUpdateRecordForm"].Executable;
            K12.Presentation.NLDPanels.Student.ListPaneContexMenu["刪除異動資料"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0)
                {
                    DeleteUpdateRecordForm durf = new DeleteUpdateRecordForm(K12.Presentation.NLDPanels.Student.SelectedSource);
                    durf.ShowDialog();
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("請選擇學生");
                }
            };
        }

        static void btnChangeInfo_Click(object sender, EventArgs e)
        {
            new UpdateRecordModule_IBSH.Wizards.NewChangeInfoProcess(new AccessHelper().StudentHelper.GetSelectedStudent()[0].StudentID).ShowDialog();
        }

        //static void btnChangeDept_Click(object sender, EventArgs e)
        //{
        //    new UpdateRecordModule_SH_D.Wizards.ChangeDeptProcess(new AccessHelper().StudentHelper.GetSelectedStudent()[0].StudentID).ShowDialog();
        //}

    }
}
