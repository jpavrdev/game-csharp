using System;

namespace JogoDeTurnos
{
    // Classe que representa qualquer combatente (Herói ou Monstro)
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
            VidaMaxima = vida; // Para não curar além do máximo
            Ataque = ataque;
            PocaoCura = 2; // Começa com 2 poções
            Mana = mana;
            ManaMaxima = mana; // Não pode usar magia se for menor

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
            // Setup Inicial
            Console.Title = "RPG de Console - Batalha Épica";
            Random dado = new Random();

            Personagem heroi = new Personagem("Guerreiro", 100, 15, 15);
            Personagem monstro = new Personagem("Orc", 80, 10, 20);

            Console.WriteLine($"Um {monstro.Nome} selvagem apareceu!");

            // Game Loop (Loop Principal)
            while (heroi.EstaVivo() && monstro.EstaVivo())
            {
                Console.WriteLine("\n--- SEU TURNO ---");
                Console.WriteLine($"{heroi.Nome} (HP: {heroi.Vida}/{heroi.VidaMaxima}) vs {monstro.Nome} (HP: {monstro.Vida})");
                Console.WriteLine("Escolha: (A) Atacar (B) Curar (C) Magia?");
                
                string escolha = Console.ReadLine().ToUpper();

                // Lógica do Jogador
                if (escolha == "A")
                {
                    // Dano variável (Ataque + aleatório entre -2 e 3)
                    int dano = heroi.Ataque + dado.Next(-2, 4); 
                    monstro.Vida -= dano;
                    Console.WriteLine($"Você atacou o {monstro.Nome} e causou {dano} de dano!");
                }
                else if (escolha == "C")
                {
                    if (heroi.Mana >= heroi.ManaMaxima && heroi.Mana > 0) {
                        int danoMagia = heroi.Mana + dado.Next(-3, 8);
                        heroi.Mana -= 10;
                        monstro.Vida -= danoMagia;
                        Console.WriteLine($"Você atacou o {monstro.Nome} com MAGIA e causou {danoMagia} de dano!!");
                    } else {
                        Console.WriteLine("Você não tem mais mana!");
                    }

                }
                else if (escolha == "B")
                {
                    if (heroi.PocaoCura > 0)
                    {
                        int cura = 25;
                        heroi.Vida += cura;
                        if (heroi.Vida > heroi.VidaMaxima) heroi.Vida = heroi.VidaMaxima;
                        heroi.PocaoCura--;
                        Console.WriteLine($"Você bebeu uma poção. Recuperou {cura} HP. Restam {heroi.PocaoCura} poções.");
                    }
                    else
                    {
                        Console.WriteLine("Você não tem mais poções! Perdeu o turno procurando na mochila...");
                    }
                }
                else
                {
                    Console.WriteLine("Comando inválido! Você tropeçou e perdeu a vez.");
                }

                // Checagem de Vitória Imediata
                if (!monstro.EstaVivo()) break;

                // Turno do Inimigo
                System.Threading.Thread.Sleep(1000); 
                
                Console.WriteLine("\n--- TURNO DO INIMIGO ---");
                int decisaoMonstro = dado.Next(0,2);

                int danoInimigo = 0;

                if (decisaoMonstro == 0) {
                    // --- ATAQUE FÍSICO ---
                    danoInimigo = monstro.Ataque + dado.Next(-1, 3);
                    Console.WriteLine($"O {monstro.Nome} avançou e te mordeu!");
                    heroi.Vida -= danoInimigo;
                    Console.WriteLine($"Você recebeu {danoInimigo} de dano!");
                } else {
                    // --- ATAQUE MAGICO ---
                    if(monstro.Mana <= monstro.ManaMaxima && monstro.Mana > 0) {                        
                        danoInimigo = monstro.Mana + dado.Next(-1, 3);
                        monstro.Mana -= 10;
                        Console.WriteLine($"O {monstro.Nome} conjurou uma magia sombria!");
                        heroi.Vida -= danoInimigo;
                        Console.WriteLine($"Você recebeu {danoInimigo} de dano!");
                    } else {
                        Console.WriteLine($"O {monstro.Nome} ficou sem mana!");
                    }
                }
            }

            // Fim de Jogo
            Console.WriteLine("\n--------------------------");
            if (heroi.EstaVivo())
            {
                Console.WriteLine($"VITÓRIA! O {monstro.Nome} foi derrotado!");
            }
            else
            {
                Console.WriteLine("DERROTA... Seu herói caiu em combate.");
            }
            
            Console.ReadKey();
        }
    }
}