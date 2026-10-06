import { ChevronLeft, ChevronRight } from 'lucide-react';

interface PaginationProps {
  currentPage: number;
  pageCount: number;
  firstItem: number;
  lastItem: number;
  totalItems: number;
  itemLabel?: string;
  onPageChange: (page: number) => void;
}

export default function Pagination({
  currentPage,
  pageCount,
  firstItem,
  lastItem,
  totalItems,
  itemLabel = 'items',
  onPageChange,
}: PaginationProps) {
  return (
    <div className="flex flex-col gap-3 border-t border-slate-100 px-5 py-4 text-sm text-slate-500 sm:flex-row sm:items-center sm:justify-between">
      <span>
        Showing {firstItem} to {lastItem} of {totalItems} {itemLabel}
      </span>
      <div className="flex items-center gap-1">
        <button
          type="button"
          aria-label="Previous page"
          disabled={currentPage === 1}
          onClick={() => onPageChange(Math.max(1, currentPage - 1))}
          className="rounded-lg border border-slate-200 p-2 text-slate-600 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40"
        >
          <ChevronLeft size={16} />
        </button>
        {Array.from({ length: pageCount }, (_, index) => index + 1).map(
          (pageNumber) => (
            <button
              key={pageNumber}
              type="button"
              onClick={() => onPageChange(pageNumber)}
              className={`size-9 rounded-lg text-sm font-medium ${pageNumber === currentPage ? 'bg-blue-600 text-white' : 'text-slate-600 hover:bg-slate-100'}`}
            >
              {pageNumber}
            </button>
          ),
        )}
        <button
          type="button"
          aria-label="Next page"
          disabled={currentPage === pageCount}
          onClick={() => onPageChange(Math.min(pageCount, currentPage + 1))}
          className="rounded-lg border border-slate-200 p-2 text-slate-600 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40"
        >
          <ChevronRight size={16} />
        </button>
      </div>
    </div>
  );
}
