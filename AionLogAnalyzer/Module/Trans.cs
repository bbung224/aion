using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AionLogAnalyzer
{
    public class Trans
    {
        String[] table = {
"jkhinolmrspqvwtuzGbcJafgde",
"11111111111111111322322222",
"efcdijghmnklqropuvstyzabIJ",
"11111111111111111111112233",
"fgdejkhinolmrspqvwtuzGbcJa",
"11111111111111111111132232",
"ghefklijopmnstqrwxuvGHcdab",
"11111111111111111111332222",
"ihkjmlonqpsrutwvyxazcbedgf",
"11111111111111111121222222",
"dcfehgjilknmporqtsvuxwzyba",
"11111111111111111111111122",
"edgfihkjmlonqpsrutwvyxazcb",
"11111111111111111111112122"
    };
        private TransEntity[,] TransTable;

        public Trans()
        {
            TransTable = new TransEntity[7, 26];

            for (int i = 0; i < 7; i++)
            {
                char c = 'a';
                for (int j = 0; j < 26; j++)
                {
                    TransEntity t;
                    if (i < 4)
                    {
                        t = new TransEntity((char)(c+j), table[i*2][j], (byte)(table[i*2 + 1][j] -48));
                    } else {
                        t = new TransEntity((char)(c + j), table[i * 2][j], (byte)(table[i * 2 + 1][j] - 44));
                    }

                    TransTable[i, j] = t;
                }
            }
        }

        public String Translate(String str, bool bMa)
        {
            StringBuilder sb = new StringBuilder();
            char[] c = str.ToLower().ToCharArray();
            int startTableIndex = (bMa) ? 0 : 4;
            int nextTableIndex = startTableIndex;

            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] >= 'a' && c[i] <= 'z')
                {
                    TransEntity t = TransTable[nextTableIndex, c[i] - 'a'];
                    sb.Append(t.Type);
                    nextTableIndex = t.NextTable;
                }
                else if (c[i] >= '가' && c[i] <= '힣')
                {
                    return null;
                }
                else
                {
                    nextTableIndex = startTableIndex;
                    sb.Append(c[i]);
                }
            }
            return sb.ToString();
        }

    }


    public struct TransEntity
    {
        public char Say;
        public char Type;
        public byte NextTable;
        public TransEntity(char Say, char Type, byte NextTable)
        {
            this.Say = Say;
            this.Type = Type;
            this.NextTable = NextTable;
        }
    }
}

/*
 * For Asmodians talking to Elyos:
 Initial letter:
 If you want to say: a b c d e f g h i j k l m n o p q r s t u v w x y z
 Type..............: j k h i n o l m r s p q v w t u z G b c J a f g d e
 Then go to table..: 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 3 2 2 3 2 2 2 2 2
 {"aj1", "bk1", "ch1", "dj1", "en1", "fo1", "gl1", "hm1", "ir1", "js1", "kp1", "lq1", "mv1", "nw1", "ot1", "pu1", "qz1", "rG3", "sb2", "tc2", "uJ3", "va2", "wf2", "xg2", "yd2", "ze2"}
 
 * 
Table 1:
 If you want to say: a b c d e f g h i j k l m n o p q r s t u v w x y z
 Type..............: e f c d i j g h m n k l q r o p u v s t y z a b I J
 Then go to table..: 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 2 2 3 3
{"ae1", "bf1", "cc1", "dd1", "ei1", "fj1", "gg1", "hh1", "im1", "jn1", "kk1", "ll1", "mq1", "nr1", "oo1", "pp1", "qu1", "rv1", "ss1", "tt1", "uy1", "vz1", "wa2", "xb2", "yI3", "zJ3"}
 
  
Table 2:
 If you want to say: a b c d e f g h i j k l m n o p q r s t u v w x y z
 Type..............: f g d e j k h i n o l m r s p q v w t u z G b c J a
 Then go to table..: 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 3 2 2 3 2
{"af1", "bg1", "cd1", "de1", "ej1", "fk1", "gh1", "hi1", "in1", "jo1", "kl1", "lm1", "mr1", "ns1", "op1", "pq1", "qv1", "rw1", "st1", "tu1", "uz1", "vG3", "wb2", "xc2", "uJ3", "za2"}
 
 * 
Table 3:
 If you want to say: a b c d e f g h i j k l m n o p q r s t u v w x y z
 Type..............: g h e f k l i j o p m n s t q r w x u v G H c d a b
 Then go to table..: 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 3 3 2 2 2 2
 
For Elyos talking to Asmodians
 Initial letter:
 If you want to say: a b c d e f g h i j k l m n o p q r s t u v w x y z
 Type..............: i h k j m l o n q p s r u t w v y x a z c b e d g f
 Then go to table..: 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 2 1 2 2 2 2 2 2
 
Table 1:
 If you want to say: a b c d e f g h i j k l m n o p q r s t u v w x y z
 Type..............: d c f e h g j i l k n m p o r q t s v u x w z y b a
 Then go to table..: 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 2 2
 
Table 2:
 If you want to say: a b c d e f g h i j k l m n o p q r s t u v w x y z
 Type..............: e d g f i h k j m l o n q p s r u t w v y x a z c b
 Then go to table..: 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 2 1 2 2
 

jkhinolmrspqvwtuzGbcJafgde
11111111111111111322322222
efcdijghmnklqropuvstyzabIJ
11111111111111111111112233
fgdejkhinolmrspqvwtuzGbcJa
11111111111111111111132232
ghefklijopmnstqrwxuvGHcdab
11111111111111111111332222
ihkjmlonqpsrutwvyxazcbedgf
11111111111111111121222222
dcfehgjilknmporqtsvuxwzyba
11111111111111111111111122
edgfihkjmlonqpsrutwvyxazcb
11111111111111111111112122
}
 */
