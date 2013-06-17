using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Trabalho_Final_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Gramatica G = new Gramatica();
            G.CarregaGramatica();
            G.SimplificaProducoesSubstituemVariaveis();
        }
    }

    public class Regra
    {
        public string Gerador = " ";
        public List<string> Gerados = new List<string>();

        public Regra()
        { }

        public Regra(string gerador, List<string> gerados)
        {
            this.Gerador= gerador;
            this.Gerados = gerados;
        }

        public Regra(string regra) //cria uma regra dada sua representação em string na forma R>A,B,C.....,X,Z
        {
            string[] aux;
            aux=regra.Split('>');
            this.Gerador = aux[0];
            aux = aux[1].Split(',');
            foreach (string s in aux)
            {
                this.Gerados.Add(s);
            }
            Console.WriteLine(this.ToString());
        }

        public override string ToString()
        {
            string aux;
            aux = Gerador + ">";
            foreach (string g in this.Gerados)
                aux += g +',';

            aux = aux.Remove(aux.LastIndexOf(','));
            return aux;
        }

    }

    public class Gramatica
    {
        public HashSet<Regra> Regras = new HashSet<Regra>();
        public HashSet<string> Terminais = new HashSet<string>();
        public HashSet<string> Variaveis = new HashSet<string>();
        public string Inicial = null;

        

        public void SimplificaSimbolosInuteis()
        {
            SimplificaProducoesSubstituemVariaveis();

            SimplificaSimbolosInuteisEtapa1();

            SimplificaSimbolosInuteisEtapa2();
        }

        public void SimplificaPrduçõesVazia()
        {
            HashSet<string> Vazio = new HashSet<string>();
            bool mudou=true;
            bool flag;
            while(mudou)
            {
                mudou=false;
            foreach (Regra regra in this.Regras)
            {
                if (regra.Gerados.Contains("&"))
                {
                    mudou = true;
                    Vazio.Add(regra.Gerador);
                    this.Regras.Remove(regra);
                }
                else
                {
                    flag = true;
                    foreach (string gerado in regra.Gerados)
                    {
                        if (!Vazio.Contains(gerado))
                            flag = false;
                    }
                    if (flag)
                        Vazio.Add(regra.Gerador);
                }
            }
            }

            foreach (Regra regra in this.Regras)
            {
            }

        }

        public void SimplificaSimbolosInuteisEtapa1()
        {
            HashSet<string> VarETerm = this.Terminais;
            bool parou;
            bool pertence = false;
            do
            {
                parou = true;
                foreach (Regra regra in this.Regras)
                {
                    pertence = true;
                    foreach (string Gerado in regra.Gerados)
                    {
                        if (!VarETerm.Contains(Gerado))
                        {
                            pertence = false;
                            break;
                        }
                    }

                    if (pertence)
                    {
                        VarETerm.Add(regra.Gerador);
                        parou = false;
                    }

                }

            } while (!parou);

            foreach (Regra regra in Regras)
                if (!VarETerm.Contains(regra.Gerador))
                    VarETerm.Remove(regra.Gerador);

            Variaveis.IntersectWith(VarETerm);
        }

        public void SimplificaSimbolosInuteisEtapa2()
        { 
            HashSet<string> VarETerm = new HashSet<string>();
            VarETerm.Add(this.Inicial);
            bool parou;
            do
            {
                parou = true;
                foreach (Regra regra in this.Regras)
                {
                    if(VarETerm.Contains(regra.Gerador))
                    {
                        parou = false;
                        foreach (string Gerado in regra.Gerados)
                        {
                            VarETerm.Add(Gerado);
                        }
                    }
                }

            } while (!parou);

            foreach (Regra regra in Regras)
                if (!VarETerm.Contains(regra.Gerador))
                    VarETerm.Remove(regra.Gerador);

            Variaveis.IntersectWith(VarETerm);
            Terminais.IntersectWith(VarETerm);
        }

        public void SimplificaProducoesSubstituemVariaveis()
        {
            foreach (string ger in this.Variaveis)
            {
                HashSet<string> fecho=new HashSet<string>();
                fecho.Add(ger);
                bool parou;
                do{
                    parou=true;
                    foreach (Regra regra in this.Regras)
                    {
                        if (fecho.Contains(regra.Gerador) && regra.Gerados.Count == 1 && !fecho.Contains(regra.Gerados[0]))
                        {
                            fecho.Add(regra.Gerados[0]);
                            this.Regras.Remove(regra);
                            parou=false;
                        }
                    }
                }while(!parou);

                fecho.Remove(ger);

                foreach (Regra regra in this.Regras)
                {
                    if (fecho.Contains(regra.Gerador))
                    {
                        Regra nova = regra;
                        nova.Gerador = ger;
                        this.Regras.Add(nova);
                    }
                }
             }
        }

        public void ConverteFNC()
        {
            foreach (Regra regra in this.Regras)
            {
                if (regra.Gerados.Count >= 2)
                {
                    foreach (string gerado in regra.Gerados)
                    {
                        if (Terminais.Contains(gerado))
                        {
                            this.Variaveis.Add("C" + gerado);
                            Regra nova = new Regra();
                            nova.Gerador = "C" + gerado;
                            nova.Gerados.Add(gerado);
                            this.Regras.Add(nova);
                        }

                    }
                }

                while(regra.Gerados.Count > 2)
                {
                    Regra nova = new Regra();
                    this.Variaveis.Add(regra.Gerador + regra.Gerados.Count.ToString());
                    nova.Gerador = regra.Gerador + regra.Gerados.Count.ToString();
                    nova.Gerados.Add(regra.Gerados[0]);
                    nova.Gerados.Add(regra.Gerados[1]);
                    this.Regras.Add(nova);
                    regra.Gerados.Remove(regra.Gerados[0]);
                }
            }
        }

        public void CarregaGramatica()
        {
            using (StreamReader file = new StreamReader(@"C:\\entrada.txt"))
            {
                string Entrada;

                Entrada = file.ReadLine(); //linha que deve conter a palavra terminais
                
                //Daqui para baixo , lê terminais
                Entrada = file.ReadLine(); //pega a lina que deve conter os terminais
                foreach (string ter in ElementosConjunto(Entrada)) //para cada terminal na entrada
                {
                    this.Terminais.Add(ter); //Adiciona este terminal na gramática
                }
                Entrada = file.ReadLine();//remove a linha que deve conter a palavra Variaveis

                Entrada = file.ReadLine(); //pega a lina que deve conter as variaveis

                foreach (string var in ElementosConjunto(Entrada))//para cada variavel na entrada
                {
                    this.Variaveis.Add(var);//Adiciona esta variavel na gramática
                }
                Entrada = file.ReadLine();//remove a linha que deve conter a palavra Iniciais
                Entrada = file.ReadLine(); //pega a lina que deve conter as variaveis


                foreach (string ini in ElementosConjunto(Entrada))//para cada variavel inicial na entrada (será única)
                {
                    this.Inicial = ini;
                }
                Entrada = file.ReadLine();//remove a linha que deve conter a palavra Regras
                Entrada = file.ReadToEnd();

                foreach (string regra in ElementosListaDeConjuntos(Entrada))
                {
                    this.Regras.Add(new Regra(regra));
                    
                }
                
            }
        }

        private static string[] ElementosConjunto(string Entrada) //dada uma string que representa um conjunto retorna um arranjo de strings que contém os elementos do conjunto
        {

            Entrada = Entrada.Replace(" ","");
            Entrada = Entrada.Replace("{", "");
            Entrada = Entrada.Replace("}", "");//remove chaves e espaços
            while (Entrada.Contains('#'))
            {
                Entrada=Entrada.Remove(Entrada.IndexOf('#'));//remove os comentários
            }
            while (Entrada.Contains(';'))
            {
                Entrada=Entrada.Remove(Entrada.IndexOf(';')); //remove o que vem depois do ponto e virgula (irrelevante para nosso trabalho)
            }
            return Entrada.Split(',');
            
        }

        private static string[] ElementosListaDeConjuntos(string Entrada) //retorna os elementos de uma lista de conjuntos em que cada conjunto contem apenas um elemento
        {
            string[] aux;
            Entrada = Entrada.Replace(" ", "");
            Entrada = Entrada.Replace("{", "");
            Entrada = Entrada.Replace("}", "");//remove chaves e espaços
            aux = Entrada.Split('\n');
            for (int i = 0; i < aux.Length; i++)
            {
                while (aux[i].Contains('#'))
                {
                    aux[i] = aux[i].Remove(aux[i].IndexOf('#'));//remove os comentários
                }

                while (aux[i].Contains(';'))
                {
                    aux[i] = aux[i].Remove(aux[i].IndexOf(';')); //remove o que vem depois do ponto e virgula (irrelevante para nosso trabalho)
                }
            }
            return aux;
        }
    }
}
