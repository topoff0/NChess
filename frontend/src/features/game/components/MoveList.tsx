type MoveListProps = {
  moveNotations: string[];
};

export const MoveList = ({ moveNotations }: MoveListProps) => {
  return (
    <div className="min-h-0 flex-1 overflow-y-auto">
      {moveNotations.length === 0 ? (
        <p className="text-sm font-bold text-wood-dark/60">No moves yet</p>
      ) : (
        <ol className="grid grid-cols-2 gap-x-4 gap-y-1 text-sm font-bold text-wood-dark">
          {moveNotations.map((notation, index) => (
            <li key={index}>
              {index % 2 === 0 && <span className="mr-2 text-wood-dark/50">{index / 2 + 1}.</span>}
              {notation}
            </li>
          ))}
        </ol>
      )}
    </div>
  );
};
