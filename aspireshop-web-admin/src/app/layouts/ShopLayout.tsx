import { useState } from 'react';
import { Outlet, useLocation, useNavigate } from 'react-router';
import {
  BarChart3,
  Bell,
  CalendarDays,
  ChevronDown,
  LayoutDashboard,
  Menu,
  Package,
  Settings,
  ShoppingCart,
  Tags,
  X,
} from 'lucide-react';

type NavigationItem = {
  label: string;
  href: string;
  icon: typeof LayoutDashboard;
};

const navigationItems: NavigationItem[] = [
  { label: 'Dashboard', href: '/', icon: LayoutDashboard },
  { label: 'Products', href: '/products', icon: Package },
  { label: 'Categories', href: '/categories', icon: Tags },
  { label: 'Orders', href: '/orders', icon: ShoppingCart },
];

function isActivePath(itemPath: string, activePath: string) {
  if (itemPath === '/') {
    return activePath === '/';
  }

  return activePath === itemPath || activePath.startsWith(`${itemPath}/`);
}

export default function ShopLayout() {
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();
  const activePath = location.pathname;

  const goTo = (href: string) => {
    navigate(href);
    setIsSidebarOpen(false);
  };

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <div className="flex min-h-screen">
        <aside
          className={`fixed inset-y-0 left-0 z-40 flex w-64 flex-col bg-slate-900 text-white transition-transform duration-200 lg:static lg:translate-x-0 ${
            isSidebarOpen ? 'translate-x-0' : '-translate-x-full'
          }`}
        >
          <div className="flex h-16 items-center justify-between border-b border-white/10 px-5">
            <button
              type="button"
              className="flex items-center gap-2 text-sm font-semibold tracking-tight"
              onClick={() => goTo('/')}
            >
              <span className="grid size-8 place-items-center rounded-lg bg-blue-600">
                <ShoppingCart size={17} strokeWidth={2.5} />
              </span>
              AspireShop
            </button>
            <button
              type="button"
              aria-label="Close navigation"
              className="rounded-md p-1 text-slate-400 hover:bg-white/10 hover:text-white lg:hidden"
              onClick={() => setIsSidebarOpen(false)}
            >
              <X size={18} />
            </button>
          </div>

          <nav className="flex-1 space-y-1 px-3 py-5" aria-label="Main navigation">
            {navigationItems.map((item) => {
              const Icon = item.icon;
              const active = isActivePath(item.href, activePath);

              return (
                <button
                  key={item.href}
                  type="button"
                  onClick={() => goTo(item.href)}
                  className={`flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-left text-sm transition-colors ${
                    active
                      ? 'bg-blue-600 text-white shadow-sm'
                      : 'text-slate-300 hover:bg-white/10 hover:text-white'
                  }`}
                >
                  <Icon size={17} strokeWidth={active ? 2.3 : 2} />
                  <span>{item.label}</span>
                </button>
              );
            })}
          </nav>

          <div className="border-t border-white/10 p-3">
            <button
              type="button"
              onClick={() => goTo('/settings')}
              className={`flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-left text-sm transition-colors ${
                activePath.startsWith('/settings')
                  ? 'bg-blue-600 text-white'
                  : 'text-slate-300 hover:bg-white/10 hover:text-white'
              }`}
            >
              <Settings size={17} />
              Settings
            </button>
          </div>
        </aside>

        {isSidebarOpen && (
          <button
            type="button"
            aria-label="Close navigation overlay"
            className="fixed inset-0 z-30 bg-slate-950/50 lg:hidden"
            onClick={() => setIsSidebarOpen(false)}
          />
        )}

        <div className="flex min-w-0 flex-1 flex-col">
          <header className="flex h-16 items-center justify-between border-b border-slate-200 bg-white px-4 sm:px-6">
            <div className="flex items-center gap-3">
              <button
                type="button"
                aria-label="Open navigation"
                className="rounded-lg p-2 text-slate-600 hover:bg-slate-100 lg:hidden"
                onClick={() => setIsSidebarOpen(true)}
              >
                <Menu size={20} />
              </button>
              <div className="hidden items-center gap-2 text-sm text-slate-500 sm:flex">
                <BarChart3 size={17} />
                Store administration
              </div>
            </div>

            <div className="flex items-center gap-2 sm:gap-4">
              <button
                type="button"
                className="hidden items-center gap-2 rounded-lg border border-slate-200 px-3 py-2 text-xs font-medium text-slate-600 hover:bg-slate-50 sm:flex"
              >
                <CalendarDays size={15} />
                Last 30 days
                <ChevronDown size={14} />
              </button>
              <button
                type="button"
                aria-label="Notifications"
                className="rounded-lg border border-slate-200 p-2 text-slate-600 hover:bg-slate-50"
              >
                <Bell size={16} />
              </button>
              <button type="button" className="flex items-center gap-2 text-left">
                <span className="grid size-9 place-items-center rounded-full bg-indigo-500 text-xs font-semibold text-white">
                  JD
                </span>
                <span className="hidden leading-tight sm:block">
                  <span className="block text-xs font-semibold text-slate-800">John Doe</span>
                  <span className="block text-[11px] text-slate-500">Admin</span>
                </span>
                <ChevronDown size={14} className="hidden text-slate-500 sm:block" />
              </button>
            </div>
          </header>

          <main className="flex-1 p-4 sm:p-6 lg:p-8">
            <Outlet />
          </main>
        </div>
      </div>
    </div>
  );
}
