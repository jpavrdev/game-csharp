using System;

namespace JogoDeTurnos
{
    // --- CLASSE DO PERSONAGEM ---
    class Personagem
    {
        public string Nome { get; set; }
        public int Vida { get; set; }
        public int VidaMaxima { get; set; }
        public int Ataque { get; set; }
        public int PocaoCura { get; set; }
        public int Mana { get; set; }
        public int ManaMaxima { get; set; }
        public int Nivel { get; set; }
        public int Xp { get; set; }
        public int XpProximoNivel { get; set; }

        public Personagem(string nome, int vida, int ataque, int mana)
        {
            Nome = nome;
            Vida = vida;
            VidaMaxima = vida;
            Ataque = ataque;
            PocaoCura = 3; // Começa com 3 poções
            Mana = mana;
            ManaMaxima = mana;
            Nivel = 1;
            Xp = 0;
            XpProximoNivel = 100;
        }

        public bool EstaVivo()
        {
            return Vida > 0;
        }
    }

    // --- PROGRAMA PRINCIPAL ---
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RPG C# - A TORRE INFINITA";
            Random dado = new Random();

            // CRIAÇÃO DO HERÓI
            Console.WriteLine("Qual o nome do seu Herói?");
            string nomeHeroi = Console.ReadLine() ?? "Aventureiro";

            Personagem heroi = new Personagem(nomeHeroi, 150, 12, 30);
            
            int monstrosDerrotados = 0;
            bool jogoRodando = true;

            // --- LOOP DA CAMPANHA ---
            while (jogoRodando && heroi.EstaVivo())
            {
                monstrosDerrotados++;

                // Lógica de Dificuldade: Inimigos ficam mais fortes
                int vidaMonstro = 50 + (monstrosDerrotados * 12);
                int ataqueMonstro = 8 + (monstrosDerrotados * 2);
                int manaMonstro = 10 + (monstrosDerrotados * 5);

                // Sorteio do Nome do Monstro
                string[] nomes = { "Goblin", "Orc", "Esqueleto", "Troll", "Bandido", "Dragão Jovem" };
                string nomeSorteado = nomes[dado.Next(0, nomes.Length)];

                // Lógica Especial: CHEFÃO
                if (nomeSorteado == "Dragão Jovem")
                {
                    vidaMonstro *= 2;    // Dobro de vida
                    ataqueMonstro += 5;  // Mais ataque
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("\n!!! ALERTA DE CHEFE: UM DRAGÃO APARECEU !!!");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(2000);
                }

                Personagem monstro = new Personagem($"{nomeSorteado} Lvl {monstrosDerrotados}", vidaMonstro, ataqueMonstro, manaMonstro);

                // --- LOOP DA BATALHA ---
                while (heroi.EstaVivo() && monstro.EstaVivo())
                {
                    Console.Clear(); // Limpa a tela a cada turno
                    Console.WriteLine($"=== SALA {monstrosDerrotados} ===");
                    Console.WriteLine("\n-------------------------------------------------");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($" {heroi.Nome.PadRight(15)} Lvl {heroi.Nivel}  | HP: {heroi.Vida}/{heroi.VidaMaxima} | MP: {heroi.Mana}/{heroi.ManaMaxima} | Pots: {heroi.PocaoCura}");
                    Console.ResetColor();
                    Console.WriteLine($"       VS");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" {monstro.Nome.PadRight(15)} | HP: {monstro.Vida}     | MP: {monstro.Mana}");
                    Console.ResetColor();
                    Console.WriteLine("-------------------------------------------------");
                    
                    Console.WriteLine("\nEscolha sua ação:");
                    Console.WriteLine("[A] Atacar (Físico)");
                    Console.WriteLine("[M] Magia (Custo: 5 MP)");
                    Console.WriteLine("[C] Curar (Beber Poção)");
                    Console.Write("> ");
                    
                    string escolha = (Console.ReadLine() ?? "Z").ToUpper();
                    Console.WriteLine(); // Pula linha

                    // 1. TURNO DO JOGADOR
                    if (escolha == "A")
                    {
                        int resultado = RolarD20(heroi.Nome, heroi.Ataque, dado, false);
                        
                        if (resultado > 0) 
                            monstro.Vida -= resultado;
                        else if (resultado < 0) // Auto-Dano
                            heroi.Vida += resultado; // Soma o negativo
                    }
                    else if (escolha == "M")
                    {
                        int custo = 5;
                        if (heroi.Mana >= custo)
                        {
                            heroi.Mana -= custo;
                            int resultado = RolarD20(heroi.Nome, 20 + (monstrosDerrotados * 2), dado, true); // Magia escala com nível
                            
                            if (resultado > 0) 
                                monstro.Vida -= resultado;
                            else if (resultado < 0) 
                                heroi.Vida += resultado;
                        }
                        else
                        {
                            Console.WriteLine("Mana insuficiente! Perdeu o turno tentando concentrar...");
                        }
                    }
                    else if (escolha == "C")
                    {
                        if (heroi.PocaoCura > 0)
                        {
                            int cura = 40;
                            heroi.Vida += cura;
                            if (heroi.Vida > heroi.VidaMaxima) heroi.Vida = heroi.VidaMaxima;
                            heroi.PocaoCura--;
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"Glup! Recuperou {cura} HP.");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine("Sem poções!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Você hesitou e perdeu a vez!");
                    }

                    // Checa se o monstro morreu antes de ele atacar
                    if (!monstro.EstaVivo()) break;

                    Console.WriteLine("\n--- Turno do Inimigo ---");
                    System.Threading.Thread.Sleep(1000);

                    // IA: 30% de chance de usar magia se tiver mana
                    bool monstroUsaMagia = (monstro.Mana >= 5 && dado.Next(0, 100) < 30);
                    int danoInimigo = 0;

                    if (monstroUsaMagia)
                    {
                        Console.WriteLine($"O {monstro.Nome} prepara um feitiço...");
                        monstro.Mana -= 5;
                        danoInimigo = RolarD20(monstro.Nome, ataqueMonstro + 5, dado, true);
                    }
                    else
                    {
                        Console.WriteLine($"O {monstro.Nome} ataca furiosamente!");
                        danoInimigo = RolarD20(monstro.Nome, ataqueMonstro, dado, false);
                    }

                    // Aplica dano do inimigo
                    if (danoInimigo > 0)
                    {
                        heroi.Vida -= danoInimigo;
                    }
                    else if (danoInimigo < 0)
                    {
                        monstro.Vida += danoInimigo; // Monstro se fere
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"O inimigo errou feio e se machucou!");
                        Console.ResetColor();
                    }

                    Console.WriteLine("\n[Pressione ENTER para continuar]");
                    Console.ReadLine(); // Pausa para ler o combate
                }

                // --- PÓS-BATALHA ---
                if (heroi.EstaVivo())
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nVITÓRIA! O {monstro.Nome} foi eliminado!");
                    Console.ResetColor();

                    int xpGanho = 50 + (monstrosDerrotados * 10);
                    heroi.Xp += xpGanho;
                    Console.WriteLine($"Você ganhou {xpGanho} XP!");

                    if (heroi.Xp >= heroi.XpProximoNivel) {
                        heroi.Nivel++;
                        heroi.Xp -= heroi.XpProximoNivel; // Sobra o resto do XP
                        heroi.XpProximoNivel += 50; // Próximo nível é mais difícil

                        // Melhora os status
                        heroi.VidaMaxima += 20;
                        heroi.ManaMaxima += 10;
                        heroi.Ataque += 2;
                        
                        // Cura total ao upar
                        heroi.Vida = heroi.VidaMaxima;
                        heroi.Mana = heroi.ManaMaxima;

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"\nLEVEL UP! Você alcançou o nível {heroi.Nivel}!");
                        Console.WriteLine($"Seus status aumentaram! Vida Max: {heroi.VidaMaxima} | Ataque: {heroi.Ataque}");
                        Console.ResetColor();
                        }

                    // Recompensas
                    Console.WriteLine("Você descansou brevemente...");
                    heroi.Vida += 20; 
                    if(heroi.Vida > heroi.VidaMaxima) heroi.Vida = heroi.VidaMaxima;
                    heroi.Mana += 10;
                    if(heroi.Mana > heroi.ManaMaxima) heroi.Mana = heroi.ManaMaxima;

                    // Chance de Loot (50%)
                    if (dado.Next(0, 100) < 50)
                    {
                        heroi.PocaoCura++;
                        Console.WriteLine("Você encontrou uma Poção de Cura nos espólios!");
                    }

                    Console.WriteLine("\nDeseja avançar para a próxima sala? (S/N)");
                    string continuar = (Console.ReadLine() ?? "").ToUpper();
                    if (continuar == "N") jogoRodando = false;
                }
            }

            // TELA FINAL
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n=== GAME OVER ===");
            Console.ResetColor();
            Console.WriteLine($"Você morreu bravamente.");
            Console.WriteLine($"Monstros derrotados: {monstrosDerrotados - 1}");
            Console.ReadKey();
        }

        // --- FUNÇÃO DO DADO: Retorna valor positivo, para dano do inimigo ou valor negativo, para dano em si mesmo
        static int RolarD20(string nome, int poderBase, Random dado, bool ehMagia)
        {
            int d20 = dado.Next(1, 21);
            
            Console.Write($"{nome} rolou D20 [{d20}] -> ");

            int danoFinal = 0;

            if (d20 == 20) // ACERTO CRÍTICO
            {
                danoFinal = poderBase * 2;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("CRÍTICO! DANO DOBRADO!");
                Console.ResetColor();
            }
            else if (d20 >= 15) // FORTE
            {
                danoFinal = poderBase + (d20 / 4);
                Console.WriteLine("Golpe Brutal!");
            }
            else if (d20 >= 8) // NORMAL
            {
                danoFinal = poderBase;
                Console.WriteLine("Acerto normal.");
            }
            else if (d20 >= 2) // RASPÃO
            {
                danoFinal = poderBase / 2;
                Console.WriteLine("Raspão... Dano reduzido.");
            }
            else // FALHA CRÍTICA
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (ehMagia)
                    Console.WriteLine("A magia explodiu na cara!");
                else
                    Console.WriteLine("Tropeçou e caiu na própria arma!");
                Console.ResetColor();

                return -(poderBase / 3);
            }

            if (danoFinal > 0) Console.WriteLine($"Causou {danoFinal} de dano.");
            
            return danoFinal;
        }
    }
}