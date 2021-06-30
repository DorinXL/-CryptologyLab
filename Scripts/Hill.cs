using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Hill: MonoBehaviour
{
    //定义一些常变量
    public static int M = 26;   //定义集合{a,b,...,z}的26个英文字母

    //行和列均为5
    public static int ROW = 5;
    public static int COL = 5;

    //定义5*5的加密矩阵
    static int[,] K = new int[ROW, COL];

    //定义5*5的解密矩阵
    static int[,] D = new int[ROW, COL];

    static int[] P = new int[ROW];  //明文单元
    static int[] C = new int[ROW];  //密文单元
    static int[] F = new int[ROW];  //密文解密后的单元

    //三元组gcd(a,b) = ax + by = d
    public struct GCD
    {
        public int x;
        public int y;
        public int d;
    };

    public Button backBtn, encodeBtn, matrixBtn;
    public InputField miyao, ming, mi, aftermi, mingwen;
    public GameObject HillPanel;
    public GameObject selfText;
    private void Start()
    {
        miyao.text = "";
        ming.text = "";
        mi.text = "";
        aftermi.text = "";
        mingwen.text = "";
        backBtn.onClick.AddListener(back);
        encodeBtn.onClick.AddListener(encode);
        matrixBtn.onClick.AddListener(matrix);
    }

    void encode()
    {
        string plaintext = "";
        if(miyao.text == "")
        {
            miyao.text = "请先生成随机密钥！";
            return;
        }
        if(mingwen.text == "")
        {
            mingwen.text = "请输入5元明文";
            return;
        }
        else
        {
            plaintext = mingwen.text;
        }
        string ciphertext;
        ciphertext = encryption(plaintext);
        aftermi.text = ciphertext;

        string tmp = "";
        for (int i = 0; i < ROW; i++)
        {
            tmp += (P[i] + " ");
        }
        ming.text = tmp;
        tmp = "";
        for (int i = 0; i < ROW; i++)
        {
            tmp += (C[i] + " ");
        }
        mi.text = tmp;



    }

    void back()
    {
        HillPanel.SetActive(false);
        selfText.SetActive(true);
    }

    void matrix()
    {
        string tmp = random_Matrix();
        while (!Inverse(K))
        {
            //Console.WriteLine("该矩阵模26不可逆,不可以作为密钥!");
            //Console.WriteLine();
            tmp = random_Matrix();
        }
        miyao.text = tmp;
    }

    //产生随机矩阵
    string random_Matrix()
    {
        int i, j;
        string matrix = "";
        Random rnd = new Random();
        for (i = 0; i < ROW; i++)
        {
            for (j = 0; j < COL; j++)
            {
                K[i, j] = rnd.Next(26);  //产生一个5*5模26的矩阵
            }
        }
        //Console.WriteLine("随机产生5*5的矩阵:");
        for (i = 0; i < ROW; i++)
        {
            for (j = 0; j < COL; j++)
            {
                matrix += (K[i, j] + " ");
            }
            matrix += '\n';
        }
        return matrix;
    }

    //求矩阵的行列式
    int Det(int[,] matrix, int row)
    {
        int i, j;
        int[,] cofa = new int[ROW, ROW];            //用于存放余子阵
        int l;   //l为所递归的余子阵的行
        int p = 0, q = 0;
        int sum = 0;
        //由于行和列相同(方阵),所以行列式的值一定存在,故不需要判断是否为方阵
        //递归基
        if (row == 1)
        {
            return matrix[0, 0];
        }
        for (i = 0; i < row; i++)
        {
            for (l = 0; l < row - 1; l++)
            {
                if (l < i)
                    p = 0;
                else
                    p = 1;
                for (j = 0; j < row - 1; j++)
                {
                    cofa[l, j] = matrix[l + p, j + 1];
                }
            }
            //相当于(-1)^i
            if (i % 2 == 0)
                q = 1;
            else
                q = (-1);
            sum = sum + matrix[i, 0] * q * Det(cofa, row - 1);
        }
        return sum;
    }

    //求两个数的最大公约数
    int gcd(int a, int b)
    {
        int temp;
        //交换两个数的大小,使得a为较大数
        if (a < b)
        {
            temp = a;
            a = b;
            b = temp;
        }
        while (a % b == 1)
        {
            temp = b;
            b = a % b;
            a = temp;
        }
        return b;
    }

    /*
     *判断矩阵K是否在模26的情况下可逆
     *因为矩阵在模26的情形下存在可逆矩阵的充分必要条件是
     *gcd(det K,26) = 1
     */
    bool Inverse(int[,] matrix)
    {
        if (gcd(Det(matrix, ROW), M) == 1)
            return true;
        else
            return false;
    }

    void multiphy(int[,] matrix, int[] p, int row)
    {
        int i, j;
        //先将密文单元清零
        for (int k = 0; k < C.Length; k++)
        {
            C[k] = 0;
        }
        for (i = 0; i < ROW; i++)
        {
            for (j = 0; j < ROW; j++)
            {
                C[i] += P[j] * K[j, i];
            }
        }
    }

    //将明文加密为密文
    string encryption(string plaintext)
    {
        int i;
        string ciphertext = "";
        //将字符串转化为明文数组
        for (i = 0; i < ROW; i++)
        {
            P[i] = plaintext[i] - 'a';
        }
        multiphy(K, P, ROW);
        //将密文数组转化为密文
        for (i = 0; i < ROW; i++)
        //这里先将其模26,再翻译为对应的字母
        {
            C[i] = Mod(C[i]);
            ciphertext += (char)(C[i] + 'A');
        }
        return ciphertext;
    }

    //求出伴随矩阵
    void adjoint_matrix(int[,] matrix, int row)
    {
        int i, j, k, l;
        int p, q;
        p = q = 0;
        int[,] temp = new int[ROW, ROW];
        for (i = 0; i < ROW; i++)
        {
            for (j = 0; j < ROW; j++)
            {
                for (k = 0; k < ROW - 1; k++)
                {
                    if (k < i)
                        p = 0;
                    else
                        p = 1;
                    for (l = 0; l < ROW - 1; l++)
                    {
                        if (l < j)
                            q = 0;
                        else
                            q = 1;
                        temp[k, l] = matrix[k + p, l + q];
                    }
                }
                D[j, i] = (int)Math.Pow(-1, (double)i + j) * Det(temp, ROW - 1);
                D[j, i] = Mod(D[j, i]);
            }
        }
    }

    //将密文解密为明文(为了辨识清楚,我们统一以小写字母作为明文,大写字母作为密文)
    string deciphering(string ciphertext)
    {
        //求出矩阵的逆
        string text = "";
        int determinant = Det(K, ROW);
        int inver = inverse(determinant, 26);
        adjoint_matrix(K, ROW);   //伴随矩阵
        Console.WriteLine("行列式的值: " + determinant);
        int i, j;
        for (int k = 0; k < F.Length; k++) F[k] = 0;
        for (i = 0; i < ROW; i++)
        {
            for (j = 0; j < ROW; j++)
            {
                F[i] += C[j] * D[j, i];
            }
            F[i] *= inver;
            F[i] = Mod(F[i]);   //算到的结果要模去26
        }
        for (i = 0; i < ROW; i++)
            text += (char)(F[i] + 'a');
        return text;
    }

    GCD extended_Euclid(int a, int b)
    {
        GCD aa = new GCD();
        GCD bb = new GCD();
        if (b == 0)
        {
            aa.x = 1;
            aa.y = 0;
            aa.d = a;
            return aa;
        }
        else
        {
            bb = extended_Euclid(b, a % b);
            aa.x = bb.y;
            aa.y = bb.x - (a / b) * bb.y;
            aa.d = bb.d;
        }
        return aa;
    }

    int inverse(int a, int m)
    {
        GCD aa;
        aa = extended_Euclid(a, m);
        return aa.x;
    }

    int Mod(int a)
    {
        return a >= 0 ? a % M : (M + a % M);
    }

    //static public void Main()
    //{

    //    //利用所选密钥，对给定的5元明文信息进行加解密
    //    string plaintext, ciphertext;



    //    Console.WriteLine("***输入0:退出          ***");
    //    Console.WriteLine("***输入1:查看明文空间对***");
    //    Console.WriteLine("***输入2:查看密文空间对***");
    //    Console.WriteLine("***输入3:查看密钥      ***");
    //    Console.WriteLine("***输入4:将消息解密    ***");
    //    Console.WriteLine("***输入5:查看菜单      ***");

    //        else if (c == '4')
    //        {
    //            hh.adjoint_matrix(K, ROW);
    //            string ss;
    //            ss = hh.deciphering(ciphertext);
    //            Console.WriteLine("该密文解密过后,显示的原来的明文消息:");
    //            Console.WriteLine(ss);
    //            Console.WriteLine();
    //        }
    //        else
    //        {
    //            Console.WriteLine("***输入0:退出          ***");
    //            Console.WriteLine("***输入1:查看明文空间对***");
    //            Console.WriteLine("***输入2:查看密文空间对***");
    //            Console.WriteLine("***输入3:查看密钥      ***");
    //            Console.WriteLine("***输入4:将消息解密    ***");
    //            Console.WriteLine("***输入5:查看菜单      ***");
    //        }
    //    }
    //}

}

