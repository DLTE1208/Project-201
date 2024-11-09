using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// ��Ź˿�ʹ�õĺ���
/// </summary>
 public class CustomerFunctions
{
    private string justForTest = "This is just for test";

    /// <summary>
    /// ����CNOȡ�ø��û�������Ϣ��������������ñ���
    /// </summary>
    /// <param name="_connectionString">���ݿ������ַ���</param>
    /// <param name="_CNO">�˿ͱ��</param>
    /// <param name="_name">����</param>
    /// <param name="_password">����</param>
    /// <param name="_sex">�Ա�</param>
    /// <param name="_contactWay">��ϵ��ʽ</param>
    /// <param name="_VIPRank">VIP�ȼ��������VIP���򷵻ؿ��ַ���</param>
    static public void UpdatePersonalInfo(string _connectionString, string _CNO, ref string _name, ref string _password, ref string _sex, ref string _contactWay, ref string _VIPRank)
    {
        {
            using (SqlConnection _connection = new SqlConnection(_connectionString))
            {
                try
                {
                    _connection.Open();

                    string _query = "SELECT cname, password, sex, contactway, viprank, " +
                                    "CASE WHEN DATEDIFF(DAY, upgradedate, GETDATE()) > duration OR viprank IS NULL THEN 0 ELSE 1 END " +
                                    "FROM customer AS c " +
                                    "LEFT JOIN vipcustomer AS vc ON c.cno = vc.cno " +
                                    $"WHERE c.cno = '{_CNO}'";
                    SqlCommand _command = new SqlCommand(_query, _connection);
                    SqlDataReader _reader = _command.ExecuteReader();

                    if (_reader.HasRows)
                    {
                        _reader.Read();
                        _name = _reader.GetValue(0).ToString();
                        _password = _reader.GetValue(1).ToString();
                        _sex = _reader.GetValue(2).ToString();
                        _contactWay = _reader.GetValue(3).ToString();
                        if (_reader.GetValue(5).ToString() == "1") _VIPRank = _reader.GetValue(4).ToString();
                        else _VIPRank = "��";
                    }
                    else Debug.Log("���ش���δ�������κ�����");
                }
                catch (System.Exception _ex)
                {
                    Debug.Log("�������" + _ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// ע��һ���˿��˻�������customer�����һ���µĹ˿���Ϣ
    /// </summary>
    /// <param name="_connectionString">���ݿ������ַ���</param>
    /// <param name="_name">����</param>
    /// <param name="_password">����</param>
    /// <param name="_sex">�Ա�</param>
    /// <param name="_contactway">��ϵ��ʽ</param>
    /// <returns>�˿ͱ�ţ����������"-1"</returns>
    static public string TryRegisterCustomerAccount(string _connectionString, string _name, string _password, string _sex, string _contactway)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                //�õ�δʹ�õĹ˿ͱ��
                string _newCNO = GetNewNO(_connectionString, "c");

                //ִ�����ݲ���
                string _query = string.Format("INSERT INTO customer VALUES('{0}', '{1}', '{2}', '{3}', '{4}');", _newCNO, _name, _password, _sex, _contactway);
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.ExecuteNonQuery();

                //Debug.Log("�˻�ע����ɣ��˻�ID��" + _newCNO + " �˻�ӵ���ߣ�" + _name);
                return _newCNO;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return "-1";
            }
        }
    }

    /// <summary>
    /// ��¼�˿��˻������ز���ֵ
    /// </summary>
    /// <param name="_connectionString">���ݿ������ַ���</param>
    /// <param name="_CNO">�˿ͱ��CNO����Uid</param>
    /// <param name="_password">����</param>
    /// <returns>��¼�ɹ����</returns>
    static public bool TryLoginCustomerAccount(string _connectionString, string _CNO, string _password)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("SELECT cname FROM customer WHERE cno = '{0}' AND password = '{1}'", _CNO, _password);
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows) return true;
                else return false;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// ����Ƿ��������VIP�����ز���ֵ����������������ʱ��Ϊ0����ʾ����������Ϊ-1����ʾVIP�߼���δ����ʱ������Ϊ�ͼ���Ӧ�������û���
    /// vip�������򣺣�1���ͼ���������Ϊ�߼�������ʱ�䱻���ǣ�2��ͬ��֮������������ʱ����ӣ�3���߼�����Ϊ�ͼ������ҽ���vip����ѹ���
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO">�˿ͱ��</param>
    /// <param name="_VIPRank">VIP�ȼ�</param>
    /// <param name="_duration">VIP�ĳ���ʱ��</param>
    /// <returns>�Ƿ��������VIP</returns>
    static public bool CanUpgradeVIP(string _connectionString,string _CNO, string _VIPRank, ref int _duration)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                //ִ���в����ͷ���ֵ�Ĵ洢����
                string _query = "pd4_canupgradevip";
                SqlCommand _command = new SqlCommand(_query, _connection);

                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@cno", _CNO);
                _command.Parameters.AddWithValue("@newviprank", _VIPRank);
                _command.Parameters.AddWithValue("@newduration", _duration);
                SqlParameter _outputDuration = new SqlParameter();
                _outputDuration.SqlDbType = SqlDbType.Int;
                _outputDuration.ParameterName = "@outduration";
                _outputDuration.Direction = ParameterDirection.Output;
                _command.Parameters.Add(_outputDuration);

                _command.ExecuteNonQuery();
                string _tmp = _outputDuration.Value.ToString();
                _duration = int.Parse(_tmp);

                if (_duration == -1) return false;
                else return true;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                _duration = 0;
                return false;
            }
        }
    }

    /// <summary>
    /// ����VIP�û�������VIP�˿ͱ����һ����Ϣ�������޸Ĺ���/�������ڵ���Ϣ
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO">�˿ͱ��</param>
    /// <param name="_VIPRank">VIP�ȼ�</param>
    /// <param name="_duration">VIP����ʱ��</param>
    static public void UpgradeVIPCustomer(string _connectionString, string _CNO, string _VIPRank, int _duration)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                //ִ���в����Ĵ洢����
                SqlCommand _command = new SqlCommand("pd1_setvipcustomer", _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@cno", _CNO);
                _command.Parameters.AddWithValue("@viprank", _VIPRank);
                _command.Parameters.AddWithValue("@duration", _duration);
                _command.ExecuteNonQuery();

                //Debug.Log("VIP������ɣ���ǰVIP�ȼ���" + _VIPRank);
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// �õ���������Ϣ���Զ�ά�ַ����б���ʽ���أ�ע���������isused�в��ڴ˶�ά����
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <returns>���������š�������λ�õĶ�ά�б�</returns>
    static public List<List<string>> GetDesksInfo(string _connectionString)
    {
        List<List<string>> _infoList = new List<List<string>>();

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "SELECT dno, capacity, position FROM desk";
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (!_reader.HasRows) Debug.Log("���ش���GetDesksInfo()�ж�desk��ѯ������ϢΪ��");
                while (_reader.Read())
                {
                    List<string> _tmp = new List<string>();
                    for (int _i = 0; _i < 3; _i++)
                    {
                        _tmp.Add(_reader.GetValue(_i).ToString());
                    }
                    _infoList.Add(_tmp);
                    //Debug.Log(string.Format("{0} {1} {2}", _reader.GetValue(0).ToString(), _reader.GetValue(1).ToString(), _reader.GetValue(2).ToString()));
                }
                return _infoList;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// ˢ�µõ���������isused��Ϣ��������Ҫ��ˢ�µ��ַ����б�
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_deskUsageInfo">�����б���ʾÿ�Ų����Ƿ�����ʹ�õ�</param>
    static public void UpdateDeskUsageInfo(string _connectionString, ref List<bool> _deskUsageInfo)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "SELECT isused FROM desk";
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (!_reader.HasRows) Debug.Log("���ش���GetDesksInfo()�ж�desk��ѯ������ϢΪ��");
                _deskUsageInfo.Clear();
                while (_reader.Read())
                {
                    _deskUsageInfo.Add(_reader.GetBoolean(0));
                    //Debug.Log(string.Format(_reader.GetBoolean(0).ToString()));
                }
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// �ж��Ƿ����ѡ��
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_DNO"></param>
    /// <returns></returns>
    static public bool CanChooseDesk(string _connectionString, string _DNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("SELECT isused FROM desk WHERE dno = '{0}'", _DNO);
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows) _reader.Read();
                else
                {
                    Debug.Log("���ش���CanChooseDesk()�ж�desk��ѯ������ϢΪ��");
                    return false;
                }
                if (!_reader.GetBoolean(0)) return true;
                else return false;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// ѡ��,��seating��λ������µ���Ϣ��ͬʱ��desk��������Ӧ���ӵ�isused�޸�Ϊ1
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO"></param>
    /// <param name="_DNO"></param>
    static public void ChooseDesk(string _connectionString, string _CNO, string _DNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "pd3_choosedesk";
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@cno", _CNO);
                _command.Parameters.AddWithValue("@dno", _DNO);
                _command.ExecuteNonQuery();

                //Debug.Log("ѡ����ɣ��˿�" + _CNO + "��" + _DNO + "����"); 
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// ͨ��������Ѱ�Ҷ�����
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_DNO"></param>
    /// <returns></returns>
    static public string FindOrder(string _connectionString, string _DNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = $"SELECT * FROM orders AS o LEFT JOIN historyorder AS ho ON o.ono = ho.ono WHERE dno = '{_DNO}' AND finishtime IS NULL";
                SqlCommand _command = new(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows)
                {
                    _reader.Read();
                    return _reader.GetValue(0).ToString();
                }
                else return "";
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return "";
            }
        }
    }

    /// <summary>
    /// �жϲ����Ƿ�ȫ������
    /// </summary>
    /// <param name="_deskUsageInfo"></param>
    /// <returns></returns>
    static public bool IsDeskFull(List<bool> _deskUsageInfo)
    {
        int _size = _deskUsageInfo.Count;
        for (int _i = 0; _i < _size; _i++)
        {
            if (!_deskUsageInfo[_i]) return false;
        }
        return true;
    }

    /// <summary>
    /// �����Ŷӣ���queuing���������
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO"></param>
    static public void EnterQueue(string _connectionString, string _CNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("INSERT INTO queuing VALUES('{0}', GETDATE())", _CNO);
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.ExecuteNonQuery();

                //Debug.Log("�˿�" + _CNO + "�����Ŷ�");
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// �뿪�Ŷӣ���queuing��ɾ������
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO"></param>
    static public void LeaveQueue(string _connectionString, string _CNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("DELETE FROM queuing WHERE cno = '{0}'", _CNO);
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.ExecuteNonQuery();

                //Debug.Log("�˿�" + _CNO + "�뿪�Ŷ�");
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// �кţ��õ��ղ����ı�ţ�Ȼ���queuing������һλ�˿ͣ��ж��Ƿ��Ǳ�����¼�û�������ǣ�������λ�������Ϣ����������;������ǣ��򷵻ؼ�
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO"></param>
    /// <returns></returns>
    static public bool CallNumber(string _connectionString, string _CNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("SELECT TOP 1 cno FROM queuing ORDER BY queuetime");
                SqlCommand _command1 = new SqlCommand(_query, _connection);
                SqlDataReader _reader1 = _command1.ExecuteReader();

                if (_reader1.HasRows) _reader1.Read();
                else
                {
                    Debug.Log("���ش���CallNumber()�ж�queuing��ѯ������ϢΪ��");
                    return false;
                }
                string _tmpString = _reader1.GetValue(0).ToString();
                _reader1.Close();
                if (_tmpString == _CNO)
                {
                    _query = "SELECT TOP 1 dno FROM desk WHERE isused = 0";
                    SqlCommand _command2 = new SqlCommand(_query, _connection);
                    SqlDataReader _reader2 = _command2.ExecuteReader();

                    if (_reader2.HasRows) _reader2.Read();
                    else
                    {
                        Debug.Log("���ش���CallNumber()�ж�desk���ѯ������ϢΪ��");
                        return false;
                    }
                    string _DNO = _reader2.GetValue(0).ToString();
                    LeaveQueue(_connectionString, _CNO);
                    ChooseDesk(_connectionString, _CNO, _DNO);
                    return true;

                }
                else return false;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// �õ�Ŀ�����ŶӶ��е����
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_CNO"></param>
    /// <returns>�Ŷ����</returns>
    static public string GetQueueSeq(string _connectionString, string _CNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = $"SELECT no FROM (SELECT cno, ROW_NUMBER() OVER (ORDER BY queuetime) AS no FROM queuing) AS newq WHERE cno = '{_CNO}'";
                SqlCommand _command = new(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows)
                {
                    _reader.Read();
                    return _reader.GetValue(0).ToString();
                }
                else
                {
                    Debug.Log("���ش���queuing����û�м�������Ӧcno");
                    return null;
                }
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// �õ��˵�������food���еõ����в�Ʒ��Ϣ
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <returns>������Ʒ��š����ࡢ��Ʒ�����۸�Ķ�ά�б�</returns>
    static public List<List<string>> GetFoodInfo(string _connectionString)
    {
        List<List<string>> _foodInfo = new List<List<string>>();
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "SELECT fno, kind, fname, price, img FROM food ORDER BY kind, fno";
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (!_reader.HasRows) Debug.Log("���ش���GetFoodInfo()�ж�food��ѯ������ϢΪ��");
                while (_reader.Read())
                {
                    List<string> _tmp = new List<string>();
                    for (int _i = 0; _i < 5; _i++)
                    {
                        _tmp.Add(_reader.GetValue(_i).ToString());
                    }
                    _foodInfo.Add(_tmp);
                    //Debug.Log(string.Format("{0} {1} {2} {3}", _reader.GetValue(0).ToString(), _reader.GetValue(1).ToString(), _reader.GetValue(2).ToString(), _reader.GetValue(3).ToString()));
                }
                return _foodInfo;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// �õ����Ų�Ʒ��Ϣ������bestfood���еõ��������Ų�Ʒ��Ϣ
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <returns>������Ʒ�š��Ƿ����š������������Ƿ��Ƽ����Ƿ���Ʒ�Ķ�ά�б�</returns>
    static public List<List<string>> GetBestFoodInfo(string _connectionString)
    {
        List<List<string>> _bestFoodInfo = new List<List<string>>();
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "SELECT fno, ishot, hotranking, isrecommend, isnew FROM bestfood";
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (!_reader.HasRows) Debug.Log("���ش���GetBestFoodInfo()�ж�bestfood��ѯ������ϢΪ��");
                while (_reader.Read())
                {
                    List<string> _tmp = new List<string>();
                    for (int _i = 0; _i < 5; _i++)
                    {
                        _tmp.Add(_reader.GetValue(_i).ToString());
                    }
                    _bestFoodInfo.Add(_tmp);
                    //Debug.Log(string.Format("{0} {1} {2} {3} {4}", _reader.GetValue(0).ToString(), _reader.GetValue(1).ToString(), _reader.GetValue(2).ToString(), _reader.GetValue(3).ToString(), _reader.GetValue(4).ToString()));
                }
                return _bestFoodInfo;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// �õ���Ʒ��ѡ�����availablesuboption���suboption���еõ��ò�Ʒ�Ŀ�ѡ��ѡ��
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_FNO"></param>
    /// <returns>������ѡ���š����͡����֡��۸������Ķ�ά�б�</returns>
    static public List<List<string>> GetAvlSuboption(string _connectionString, string _FNO)
    {
        List<List<string>> _avlSuboptionList = new List<List<string>>();
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("SELECT asup.sno, kind, sname, priceoffset FROM availablesuboption AS asup " +
                                              "JOIN suboption AS sub ON asup.sno = sub.sno " +
                                              "WHERE fno = '{0}' ORDER BY kind, asup.sno", _FNO);
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                //if (!_reader.HasRows) Debug.Log("���ش���GetAvailableSuboption()�ж�availablesuboption��suboption��ѯ������ϢΪ��");

                while (_reader.Read())
                {
                    List<string> _tmp = new List<string>();
                    for (int _i = 0; _i < 4; _i++)
                    {
                        _tmp.Add(_reader.GetValue(_i).ToString());
                    }
                    _avlSuboptionList.Add(_tmp);
                    //Debug.Log(string.Format("{0} {1} {2} {3}", _reader.GetValue(0).ToString(), _reader.GetValue(1).ToString(), _reader.GetValue(2).ToString(), _reader.GetValue(3).ToString()));
                }
                return _avlSuboptionList;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// ���ɶ������˿�ѡ���ɹ����Զ����ɶ���
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_DNO"></param>
    /// <returns>�����ŵ��ַ���</returns>
    static public string CreateOrder(string _connectionString, string _DNO)
    {
        string _ONO, _discount;

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                //���ɶ�����
                _ONO = GetNewNO(_connectionString, "o");

                //�õ��ۿ�
                _discount = CalculateDiscount(_connectionString, _DNO);
                string _query = string.Format("INSERT INTO orders VALUES('{0}', '{1}', GETDATE(), 0, {2})", _ONO, _DNO, _discount.ToString());
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.ExecuteNonQuery();

                //Debug.Log("���������� ������:" + _ONO);
                return _ONO;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// �����ۿۣ�����λ��ͬһ��λ��ȡ��VIPû�й������ۿ����Żݵ���Ϊ����ۿ۷���
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_DNO"></param>
    /// <returns>����ۿ�</returns>
    static public string CalculateDiscount(string _connectionString, string _DNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                //�����в����з���ֵ�Ĵ洢����
                string _query = "pd2_calculatediscount";
                SqlCommand _command = new SqlCommand(_query, _connection);

                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@dno", _DNO);
                SqlParameter _outputDiscount = new SqlParameter();
                _outputDiscount.ParameterName = "@discount";
                _outputDiscount.SqlDbType = SqlDbType.Float;
                _outputDiscount.Direction = ParameterDirection.Output;
                _command.Parameters.Add(_outputDiscount);

                _command.ExecuteNonQuery();
                string _discount = _outputDiscount.Value.ToString();
                return _discount;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// ��Ӳ�Ʒ���˿Ϳ����򶩵���ϸ����Ӳ�Ʒ
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_ONO"></param>
    /// <param name="_FNO"></param>
    /// <param name="_SNO"></param>
    /// <param name="_amount"></param>
    static public void AddFood(string _connectionString, string _ONO, string _FNO, string _SNO, int _amount)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = string.Format("INSERT INTO orderdetail VALUES('{0}', '{1}', '{2}', {3}, 1)", _ONO, _FNO, _SNO, _amount);
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.ExecuteNonQuery();

                //Debug.Log("��Ʒ�����������");
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// �õ���Ʒ����Ϣ��Ϣ�����ڴ�ӡ
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_FNO"></param>
    /// <param name="_SNO"></param>
    /// <returns></returns>
    static public List<string> GetFoodDetailInfo(string _connectionString, string _FNO, string _SNO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query;
                if (_SNO != "s0000")
                {
                    _query = $"SELECT fname, price+ priceoffset, f.kind, img, sname " +
                                $"FROM food AS f " +
                                $"LEFT JOIN availablesuboption AS avls ON avls.fno = f.fno " +
                                $"LEFT JOIN suboption AS s ON s.sno = avls.sno " +
                                $"WHERE f.fno = '{_FNO}' AND s.sno = '{_SNO}'";
                }
                else _query = $"SELECT fname, price, kind, img FROM food WHERE fno = '{_FNO}'";

                SqlCommand _command = new(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (!_reader.HasRows)
                { 
                    Debug.Log("���ش���δ��������Ӧfno��sno��Ԫ��");
                    return null;
                }
                else
                {
                    _reader.Read();

                    List<string> _returnList = new();
                    _returnList.Add(_reader.GetValue(0).ToString());
                    _returnList.Add(_reader.GetValue(1).ToString());
                    _returnList.Add(_reader.GetValue(2).ToString());
                    _returnList.Add(_reader.GetValue(3).ToString());
                    if (_SNO == "s0000") _returnList.Add("��׼");
                    else _returnList.Add(_reader.GetValue(4).ToString());
                    return _returnList;
                }
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// ��ӱ�ע���˿���ordernote��ӱ�ע
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_ONO"></param>
    /// <param name="_text"></param>
    static public void AddNote(string _connectionString, string _ONO, string _text)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _NNO = GetNewNO(_connectionString, "n");
                string _query = string.Format("INSERT INTO ordernote VALUES('{0}', '{1}', '{2}')", _NNO, _ONO, _text);
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.ExecuteNonQuery();

                //Debug.Log("��ע������");
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// �õ��˵���Ϣ
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_ONO"></param>
    /// <returns></returns>
    static public List<List<string>> GetBillInfo(string _connectionString, string _ONO)
    {
        List<List<string>> _returnList = new();

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = $"SELECT fname, amount, perprice, sumprice FROM bill WHERE ONO = '{_ONO}'";
                SqlCommand _command = new(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        List<string> _tmp = new List<string>();
                        for (int _i = 0; _i < 4; _i++)
                        {
                            _tmp.Add(_reader.GetValue(_i).ToString());
                        }
                        _returnList.Add(_tmp);
                    }
                    return _returnList;
                }
                else
                {
                    Debug.Log("���ش���:bill��ѯ������Ϊ��");
                    return null;
                }
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// ���㶩�����˿ͶԶ������н��㣬�������������ʷ������ͬʱ����δ�ϵĲ�Ʒ�����ܼ۸��п۳�
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_ONO"></param>
    /// <returns>�˿�ʵ��Ӧ���۸�</returns>
    static public float FinishOrderByCustomer(string _connectionString, string _ONO)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "pd5_finishorder";
                SqlCommand _command = new SqlCommand(_query, _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@ono", _ONO);
                _command.Parameters.AddWithValue("@iscustomerfinish", 1);
                SqlParameter _outputIncome = new SqlParameter();
                _outputIncome.ParameterName = "@outincome";
                _outputIncome.SqlDbType = SqlDbType.Money;
                _outputIncome.Direction = ParameterDirection.Output;
                _command.Parameters.Add(_outputIncome);
                _command.ExecuteNonQuery();

                float _returnincome = float.Parse(_outputIncome.Value.ToString());
                return _returnincome;
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return -1.0f;
            }
        }
    }

    /// <summary>
    /// �뿪��λ,��seating��λ��ɾ�������λ����������Ϣ��ͬʱ��desk��������Ӧ���ӵ�isused�޸�Ϊ0
    /// </summary>
    /// <param name="_connectionString"></param>
    /// <param name="_DNO"></param>
    static public void LeaveDesk(string _connectionString, string _DNO)
    {
        using (SqlConnection _connection = new(_connectionString))
        {
            try
            {
                _connection.Open();

                string _query = "pd7_leavedesk";
                SqlCommand _command = new(_query, _connection);
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@dno", _DNO);
                _command.ExecuteNonQuery();
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// �������ݿ�����
    /// </summary>
    /// <param name="_connectionString"></param>
    static public void TestDBConnection(string _connectionString)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();
                Debug.Log("���ݿ����ӳɹ�");

                //ִ�в��Բ�ѯ
                string _query = "SELECT * FROM customer";
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                //��ȡ����
                Show(_reader);
                //while(_reader.Read())
                //{
                //    Debug.Log(_reader.GetValue(0).ToString() + " " + _reader.GetValue(1).ToString());
                //}
            }
            catch (System.Exception _ex)
            {
                Debug.Log("���ݿ�����ʧ��");
                Debug.Log("�������" + _ex.Message);
            }
        }
    }

    /// <summary>
    /// ��ĳ�����л��һ��δʹ�õı�Ų���������ַ�������һ�����Ϊx0001��x00000001�����һ�����Ϊx9999��x999999999
    /// </summary>
    /// <param name="_connectionString">���ݿ������ַ���</param>
    /// <param name="_tableName">����</param>
    /// <returns>���ر���ַ�������������򷵻�Wrong�ַ���</returns>
    static public string GetNewNO(string _connectionString, string _tableName)
    {
        string _query, _firstChar;
        int _size;

        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            string _newNO = new string("");
            try
            {
                _connection.Open();

                //�жϴ����ű��ñ��
                _tableName = _tableName.ToLower();
                if (_tableName == "customer" || _tableName == "c")
                {
                    _query = "SELECT MAX(cno) FROM customer";
                    _size = 8;
                    _firstChar = "c";
                }
                else if (_tableName == "desk" || _tableName == "d")
                {
                    _query = "SELECT MAX(dno) FROM desk";
                    _size = 4;
                    _firstChar = "d";
                }
                else if (_tableName == "order" || _tableName == "o")
                {
                    _query = "SELECT MAX(ono) FROM orders";
                    _size = 8;
                    _firstChar = "o";
                }
                else if (_tableName == "food" || _tableName == "f")
                {
                    _query = "SELECT MAX(fno) FROM food";
                    _size = 4;
                    _firstChar = "f";
                }
                else if (_tableName == "suboption" || _tableName == "s")
                {
                    _query = "SELECT MAX(sno) FROM suboption";
                    _size = 4;
                    _firstChar = "s";
                }
                else if (_tableName == "auditlog" || _tableName == "l")
                {
                    _query = "SELECT MAX(lno) FROM auditlog";
                    _size = 8;
                    _firstChar = "l";
                }
                else if (_tableName == "ordernote" || _tableName == "n")
                {
                    _query = "SELECT MAX(nno) FROM ordernote";
                    _size = 8;
                    _firstChar = "n";
                }
                else return "InputStringIsWrong";

                //ִ������Ų�ѯ
                SqlCommand _command = new SqlCommand(_query, _connection);
                SqlDataReader _reader = _command.ExecuteReader();

                //������
                if (!_reader.HasRows)   //���Ϊ������
                {
                    Debug.Log("hereB");
                    _newNO = _firstChar + "1".PadLeft(_size, '0');
                    return _newNO;
                }
                else
                {
                    _reader.Read();
                    if (_reader.GetValue(0).ToString() != "") _newNO = _reader.GetValue(0).ToString();  //�������Ч����
                    else _newNO = "0000";       //������ǿ����ݣ������ǿ��ַ���

                    //�Ա�Ž��д����ַ���ȥ����ʶ��ת���ͣ�����+1������ת�ַ������ϱ�ʶ��
                    int _tmpInt = 0;
                    _tmpInt = int.Parse(_newNO.Remove(0, 1));
                    _tmpInt++;
                    if (_size == 8 && _tmpInt > 99999999) return "OutOfIndex";
                    else if (_size == 4 && _tmpInt > 9999) return "OutOfIndex";
                    else
                    {
                        _newNO = _firstChar + _tmpInt.ToString().PadLeft(_size, '0');
                        return _newNO;
                    }
                }
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
                return "Wrong";
            }
        }
    }

    /// <summary>
    /// չʾ��ά�б�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list"></param>
    static public void Show<T>(List<List<T>> _list)
    {
        int _rowSize = _list.Count;
        int _columnSize;
        string _str = "";
        for (int _i = 0; _i < _rowSize; _i++)
        {
            _columnSize = _list[_i].Count;
            for (int _j = 0; _j < _columnSize; _j++)
            {
                _str += _list[_i][_j].ToString();
                _str += " ";
            }
            Debug.Log(_str);
            _str = "";
        }
    }

    /// <summary>
    /// չʾreader
    /// </summary>
    /// <param name="_reader"></param>
    static public void Show(SqlDataReader _reader)
    {
        int _columnSize = _reader.FieldCount;
        string str = "";
        while (_reader.Read())
        {
            for (int _i = 0; _i < _columnSize; _i++)
            {
                str += _reader[_i];
                str += " ";
            }
            Debug.Log(str);
            str = "";
        }
    }

    /// <summary>
    /// ��׼SQL����ģ��
    /// </summary>
    /// <param name="_connectionString"></param>
    static public void StandardSQLFunctionTemplate(string _connectionString)
    {
        using (SqlConnection _connection = new SqlConnection(_connectionString))
        {
            try
            {
                _connection.Open();

                //Do something here
            }
            catch (System.Exception _ex)
            {
                Debug.Log("�������" + _ex.Message);
            }
        }
    }
}
