using System;

namespace JogoDeTurnos
{
    class Personagem
    {
        public string Nome { get; set; }
        public int Vida { get; set; }
        public int VidaMaxima { get; set; }
        public int Ataque { get; set; }
        public int PocaoCura { get; set; }
        public int Mana { get; set; }
        public int ManaMaxima { get; set; }

        public Personagem(string nome, int vida, int ataque, int mana)
        {
            Nome = nome;
            Vida = vida;
            VidaMaxima = vida;
            Ataque = ataque;
            PocaoCura = 2;
            Mana = mana;
            ManaMaxima = mana;
        }

        public bool EstaVivo()
        {
            return Vida > 0;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RPG de Console - Batalha Épica";
            Random dado = new Random();

            Personagem heroi = new Personagem("Guerreiro", 120, 12, 20);
            Personagem monstro = new Personagem("Orc Chefe", 100, 10, 20);

            Console.WriteLine($"Um {monstro.Nome} furioso apareceu!");

            while (heroi.EstaVivo() && monstro.EstaVivo())
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine($"{heroi.Nome} (HP: {heroi.Vida}/{heroi.VidaMaxima} | MP: {heroi.Mana})"); 
                Console.WriteLine("        VS");
                Console.WriteLine($"{monstro.Nome} (HP: {monstro.Vida} | MP: {monstro.Mana})");
                Console.WriteLine("========================================");
                Console.WriteLine("Escolha: (A)tacar (M)agia (C)urar?");
                
                string escolha = Console.ReadLine().ToUpper();

                // --- TURNO DO JOGADOR ---
                if (escolha == "A")
                {
                    int resultado = RolarD20(heroi.Nome, heroi.Ataque, dado, false);
                    
                    if (resultado > 0) {
                        monstro.Vida -= resultado;
                    } 
                    else if (resultado < 0) { // Lógica de Erro Crítico Físico
                        heroi.Vida += resultado; // Soma o negativo (diminui vida)
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Você se feriu na confusão! Perdeu {Math.Abs(resultado)} de vida.");
                        Console.ResetColor();
                    }
                }
                else if (escolha == "M")
                {
                    int custoMagia = 5;
                    if(heroi.Mana >= custoMagia) {
                        heroi.Mana -= custoMagia; // Gasta a mana antes de rolar
                        
                        int resultado = RolarD20(heroi.Nome, 18, dado, true); // Dano base mágico alto
                        
                        if (resultado > 0) {
                            monstro.Vida -= resultado;
                        }
                        else if (resultado < 0) { // Lógica de Erro Crítico Mágico
                            heroi.Vida += resultado;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"A magia instável explodiu em você! Perdeu {Math.Abs(resultado)} de vida.");
                            Console.ResetColor();
                        }
                    } else {
                        Console.WriteLine("Mana insuficiente! Perdeu o turno tentando concentrar...");
                    }
                }
                else if (escolha == "C")
                {
                    if (heroi.PocaoCura > 0)
                    {
                        int cura = 30;
                        heroi.Vida += cura;
                        if (heroi.Vida > heroi.VidaMaxima) heroi.Vida = heroi.VidaMaxima;
                        heroi.PocaoCura--;
                        Console.WriteLine($"Você bebeu uma poção. Recuperou {cura} HP. Restam {heroi.PocaoCura}.");
                    }
                    else
                    {
                        Console.WriteLine("Mochila vazia! Sem poções.");
                    }
                }
                else
                {
                    Console.WriteLine("Comando inválido! Você tropeçou.");
                }

                if (!monstro.EstaVivo()) break;

                // --- TURNO DO INIMIGO ---
                Console.WriteLine("\n--- TURNO DO INIMIGO ---");
                System.Threading.Thread.Sleep(1000); 

                bool monstroUsaMagia = (monstro.Mana >= 5 && dado.Next(0, 100) < 30);
                int danoMonstro = 0;

                if (monstroUsaMagia) {
                    Console.WriteLine($"O {monstro.Nome} está preparando um feitiço...");
                    monstro.Mana -= 5;
                    danoMonstro = RolarD20(monstro.Nome, 15, dado, true);
                } else {
                    Console.WriteLine($"O {monstro.Nome} levantou a arma...");
                    danoMonstro = RolarD20(monstro.Nome, monstro.Ataque, dado, false);
                }

                // APLICA O DANO DO MONSTRO (Seja no herói ou nele mesmo)
                if (danoMonstro > 0) 
                {
                    heroi.Vida -= danoMonstro;
                }
                else if (danoMonstro < 0)
                {
                    monstro.Vida += danoMonstro; // Monstro se machuca
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"O {monstro.Nome} se atrapalhou e tomou {Math.Abs(danoMonstro)} de dano!");
                    Console.ResetColor();
                }
            }

            // Fim de Jogo
            Console.WriteLine("\n--------------------------");
            if (heroi.EstaVivo()) {
                Console.WriteLine($"VITÓRIA! O {monstro.Nome} foi derrotado!");
            }
            else {
                Console.WriteLine("DERROTA... Seu herói caiu em combate.");
            }
            
            Console.ReadKey();
        }

        static int RolarD20(string nomeAtacante, int poderBase, Random dado, bool ehMagia) {
            int d20 = dado.Next(1, 2); 
            
            Console.Write($"{nomeAtacante} rolou D20: ");

            if (d20 == 20) Console.ForegroundColor = ConsoleColor.Yellow;
            else if (d20 == 1) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{d20}]");
            Console.ResetColor();

            int danoFinal = 0;

            if (d20 == 20) // CRÍTICO
            {
                danoFinal = poderBase * 2;
                Console.WriteLine(">>> ACERTO CRÍTICO! DANO DOBRADO! <<<");
            }
            else if (d20 >= 15) // FORTE
            {
                danoFinal = poderBase + (d20 / 3);
                Console.WriteLine("Um golpe brutal e preciso!");
            }
            else if (d20 >= 8) // NORMAL
            {
                danoFinal = poderBase;
                Console.WriteLine("Acertou em cheio.");
            }
            else if (d20 >= 2) // RASPÃO
            {
                danoFinal = poderBase / 2;
                Console.WriteLine("Passou de raspão... Dano reduzido.");
            }
            else // D20 == 1 (ERRO CRÍTICO)
            {
                danoFinal = 0;
                Console.ForegroundColor = ConsoleColor.Red;
                
                if (ehMagia) {
                    Console.WriteLine($"A magia explodiu na cara de quem lançou!");
                } else {
                    Console.WriteLine($"Errou feio! Tropeçou e caiu.");
                }
                Console.ResetColor();
                
                // Retorna negativo para sinalizar auto-dano
                return -(poderBase / 2); 
            }

            if (danoFinal > 0)
                Console.WriteLine($"Resultado: Causou {danoFinal} de dano.");
            
            return danoFinal;
        }
    }
}