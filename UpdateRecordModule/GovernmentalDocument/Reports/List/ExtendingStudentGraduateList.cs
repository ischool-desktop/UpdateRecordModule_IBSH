using System.IO;
using System.Xml;
using Aspose.Cells;
using UpdateRecordModule_IBSH.BL;
using System.Linq;
using System.Collections.Generic;

namespace UpdateRecordModule_IBSH.GovernmentalDocument.Reports.List
{
    public class ExtendingStudentGraduateList : ReportBuilder
    {
        protected override void Build(XmlElement source, string location)
        {
            Workbook template = new Workbook();

            //�qResources��TemplateŪ�X��
            template.Open(new MemoryStream(Properties.Resources.ExtendingGraduatingStudentListTemplate), FileFormatType.Excel2003);

            //�n���ͪ�excel��
            Workbook wb = new Aspose.Cells.Workbook();
            wb.Open(new MemoryStream(Properties.Resources.ExtendingGraduatingStudentListTemplate), FileFormatType.Excel2003);

            Worksheet ws = wb.Worksheets[0];

            //�������j�X��row
            int next = 24;

            //����
            int index = 0;

            //�d���d��
            Range tempRange = template.Worksheets[0].Cells.CreateRange(0, 24, false);

            //�`�@�X�����ʬ���
            int count = 0;
            int totalRec = source.SelectNodes("�M��/���ʬ���").Count;

            // ���o�W�U���s���̫Ყ�ʥN�X���
            Dictionary<string,string> LastCodeDict = new Dictionary<string,string>();



            foreach (XmlNode list in source.SelectNodes("�M��"))
            {
                //���ͲM��Ĥ@��
                //for (int row = 0; row < next; row++)
                //{
                //    ws.Cells.CopyRow(template.Worksheets[0].Cells, row, row + index);
                //}
                ws.Cells.CreateRange(index, 24, false).Copy(tempRange);

                //Page
                int currentPage = 1;
                int totalPage = (list.ChildNodes.Count / 18) + 1;

                //�g�J�W�U���O
                if (source.SelectSingleNode("@���O").InnerText == "���ץͲ��~�W�U")
                    ws.Cells[index, 0].PutValue(ws.Cells[index, 0].StringValue.Replace("�����~", "�����~"));
                else
                    ws.Cells[index, 0].PutValue(ws.Cells[index, 0].StringValue.Replace("�����~", "�����~"));

                //�g�J�N��
                ws.Cells[index, 6].PutValue("�N�X�G" + source.SelectSingleNode("@�ǮեN��").InnerText + "-" + list.SelectSingleNode("@��O�N��").InnerText);

                //�g�J�զW�B�Ǧ~�סB�Ǵ��B��O
                ws.Cells[index + 2, 0].PutValue("�զW�G" + source.SelectSingleNode("@�ǮզW��").InnerText);
                ws.Cells[index + 2, 4].PutValue(source.SelectSingleNode("@�Ǧ~��").InnerText + "�Ǧ~�� ��" + source.SelectSingleNode("@�Ǵ�").InnerText + "�Ǵ�");
                ws.Cells[index + 2, 6].PutValue(list.SelectSingleNode("@��O").InnerText);

                //�g�J���
                int recCount = 0;
                int dataIndex = index + 5;
                for (; currentPage <= totalPage; currentPage++)
                {
                    //�ƻs����
                    if (currentPage + 1 <= totalPage)
                    {
                        //for (int row = 0; row < next; row++)
                        //{
                        //    ws.Cells.CopyRow(ws.Cells, row + index, row + index + next);
                        //}
                        ws.Cells.CreateRange(index + next, 24, false).Copy(tempRange);
                    }

                    //��J���
                    for (int i = 0; i < 18 && recCount < list.ChildNodes.Count; i++, recCount++)
                    {
                        //MsgBox.Show(i.ToString()+" "+recCount.ToString());
                        XmlNode rec = list.SelectNodes("���ʬ���")[recCount];
                        ws.Cells[dataIndex, 0].PutValue(rec.SelectSingleNode("@�Ǹ�").InnerText + "\n" + rec.SelectSingleNode("@�m�W").InnerText);
                        ws.Cells[dataIndex, 1].PutValue(rec.SelectSingleNode("@�ʧO�N��").InnerText.ToString());
                        ws.Cells[dataIndex, 2].PutValue(rec.SelectSingleNode("@�ʧO").InnerText);
                        string ssn = rec.SelectSingleNode("@�����Ҹ�").InnerText;
                        if (ssn == "")
                            ssn = rec.SelectSingleNode("@�����Ҹ�").InnerText;

                        if(!LastCodeDict.ContainsKey(ssn))
                            LastCodeDict.Add(ssn,rec.SelectSingleNode("@�̫Ყ�ʥN��").InnerText.ToString());

                        ws.Cells[dataIndex, 3].PutValue(Util.ConvertDateStr2(rec.SelectSingleNode("@�ͤ�").InnerText) + "\n" + ssn);
                        ws.Cells[dataIndex, 4].PutValue(rec.SelectSingleNode("@�̫Ყ�ʥN��").InnerText.ToString());
                        ws.Cells[dataIndex, 5].PutValue(Util.ConvertDateStr2(rec.SelectSingleNode("@�Ƭd���").InnerText) + "\n" + rec.SelectSingleNode("@�Ƭd�帹").InnerText);
                        ws.Cells[dataIndex, 6].PutValue(rec.SelectSingleNode("@���~�ҮѦr��").InnerText);
                        
                        //ws.Cells[dataIndex, 7].PutValue(rec.SelectSingleNode("@�Ƶ�").InnerText);
                        if(rec.SelectSingleNode("@�S�����N�X")!=null)
                            ws.Cells[dataIndex, 7].PutValue(rec.SelectSingleNode("@�S�����N�X").InnerText);

                        dataIndex++;
                        count++;
                    }

                    //�p��X�p
                    if (currentPage == totalPage)
                    {
                        ws.Cells[index + 22, 0].PutValue("�X�p");
                        ws.Cells[index + 22, 1].PutValue(list.ChildNodes.Count.ToString());
                    }

                    //����
                    ws.Cells[index + 23, 6].PutValue("�� " + currentPage + " ���A�@ " + totalPage + " ��");
                    ws.HPageBreaks.Add(index + 24, 8);

                    //���ޫ��V�U�@��
                    index += next;
                    dataIndex = index + 5;

                    //�^���i��
                    ReportProgress((int)(((double)count * 100.0) / ((double)totalRec)));
                }
            }

            Worksheet mingdao = wb.Worksheets[1];
            Worksheet mdws = wb.Worksheets[1];
            mdws.Name = "�q�l�榡";

            Range range_header = mingdao.Cells.CreateRange(0, 1, false);
            Range range_row = mingdao.Cells.CreateRange(1, 1, false);

            mdws.Cells.CreateRange(0, 1, false).Copy(range_header);

            int mdws_index = 0;



            DAL.DALTransfer DALTranser = new DAL.DALTransfer();

            // �榡�ഫ
            List<GovernmentalDocument.Reports.List.rpt_UpdateRecord> _data = DALTranser.ConvertRptUpdateRecord(source);

            // �Ƨ� (�� �Z�O�B�~�šB��O�N�X�B���ʥN�X)
            _data = (from data in _data orderby data.ClassType, data.DeptCode, data.UpdateCode select data).ToList();

            foreach (GovernmentalDocument.Reports.List.rpt_UpdateRecord rec in _data)
            {
                mdws_index++;
                //�C�W�[�@��,�ƻs�@��
                mdws.Cells.CreateRange(mdws_index, 1, false).Copy(range_row);

                //�����~�Ǧ~��
                mdws.Cells[mdws_index, 0].PutValue(rec.ExpectGraduateSchoolYear);

                //�Z�O
                mdws.Cells[mdws_index, 1].PutValue(rec.ClassType);
                //��O�N�X
                mdws.Cells[mdws_index, 2].PutValue(rec.DeptCode);

                // 2 ��W�����O�A�ШϥΪ̦۶� 

                //�Ǹ�
                mdws.Cells[mdws_index, 4].PutValue(rec.StudentNumber);
                //�m�W
                mdws.Cells[mdws_index, 5].PutValue(rec.Name);
                //�����Ҧr��
                mdws.Cells[mdws_index, 6].PutValue(rec.IDNumber);

                //��1
                mdws.Cells[mdws_index, 7].PutValue(rec.Comment1);

                //�ʧO�N�X
                mdws.Cells[mdws_index, 8].PutValue(rec.GenderCode);
                //�X�ͤ��
                mdws.Cells[mdws_index, 9].PutValue(rec.Birthday);

                //�S�����N�X
                mdws.Cells[mdws_index, 10].PutValue(rec.SpecialStatusCode);

                //���ʭ�]�N�X
                if(LastCodeDict.ContainsKey(rec.IDNumber))
                    mdws.Cells[mdws_index, 11].PutValue(LastCodeDict[rec.IDNumber]);
                else
                    mdws.Cells[mdws_index, 11].PutValue(rec.UpdateCode);

                //�Ƭd��r
                mdws.Cells[mdws_index, 12].PutValue(rec.LastADDoc);
                //�Ƭd�帹
                mdws.Cells[mdws_index, 13].PutValue(rec.LastADNum);

                //�Ƭd���
                mdws.Cells[mdws_index, 14].PutValue(rec.LastADDate);

                //���~�ҮѦr��
                mdws.Cells[mdws_index, 15].PutValue(rec.GraduateCertificateNumber);

                //�Ƶ�����
                mdws.Cells[mdws_index, 16].PutValue(rec.Comment);

            }

            //�x�s
            wb.Save(location, FileFormatType.Excel2003);
        }

        public override string Copyright
        {
            get { return "IntelliSchool"; }
        }

        public override string Description
        {
            get { return "�����줽��95�~11��s�L�޲z��U�W�d�榡"; }
        }

        public override string ReportName
        {
            get { return "���ץͲ��~�W�U"; }
        }

        public override string Version
        {
            get { return "1.0.0.0"; }
        }
    }
}
