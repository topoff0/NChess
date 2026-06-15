type GamePageProps = {
  onLogout: () => void;
};

export const GamePage = ({ onLogout }: GamePageProps) => {
  return (
    <main className="min-h-screen bg-cream px-6 py-8 text-wood-dark">
      <header className="mx-auto flex max-w-5xl items-center justify-between">
        <h1 className="text-4xl font-black">NChess</h1>
        <button
          className="rounded-2xl border-4 border-wood-dark bg-lime
        px-5 py-2 font-bold text-wood-dark disabled:opacity-60"
          type="button"
          onClick={onLogout}>
          Logout
        </button>
      </header>
    </main>
  );
};
