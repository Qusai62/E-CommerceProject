import { Link } from '@tanstack/react-router';
import { MapPin, PenLine, Plus, Star, Trash2 } from 'lucide-react';
import {
  useGetAddressesQuery,
  useDeleteAddressMutation,
  useSetDefaultAddressMutation,
} from '@/shared/api/generated/api.generated';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { ConfirmAction } from '@/components/ui/alert-dialog';
import { EmptyState, ErrorState, FormError, LoadingState } from '@/components/ui/feedback';
import { PageHeader } from '@/components/layout/site-layout';
import { AccountNav, SignInRequired } from '../components/account-nav';

export function AddressesPage() {
  const query = useGetAddressesQuery();
  const [remove, removeState] = useDeleteAddressMutation();
  const [makeDefault, defaultState] = useSetDefaultAddressMutation();
  if (query.isLoading) return <LoadingState />;
  if (query.isError && 'status' in query.error && query.error.status === 401) {
    return <SignInRequired />;
  }
  if (query.isError) {
    return (
      <div className="page-container py-16">
        <ErrorState error={query.error} onRetry={() => void query.refetch()} />
      </div>
    );
  }
  return (
    <div className="page-container py-12">
      <PageHeader
        eyebrow="YOUR ACCOUNT"
        title="Saved addresses"
        description="Manage delivery addresses from one place."
        action={
          <Button asChild>
            <Link to="/account/addresses/new" search={{ returnTo: undefined }}>
              <Plus size={17} /> New address
            </Link>
          </Button>
        }
      />
      <div className="mt-9 grid gap-7 lg:grid-cols-[260px_1fr]">
        <AccountNav />
        <section aria-label="Saved addresses">
          <FormError error={removeState.error || defaultState.error} />
          {!query.data?.length ? (
            <EmptyState
              icon={<MapPin size={30} />}
              title="No saved addresses"
              description="Add an address to have it ready when checkout is available."
            />
          ) : (
            <div className="grid gap-4 md:grid-cols-2">
              {query.data.map((item) => (
                <Card key={item.savedAddressId} className="p-6">
                  <div className="flex items-center justify-between">
                    <MapPin className="text-moss" />
                    {item.isDefault && (
                      <Badge className="bg-blue-50 text-blue-700">Default address</Badge>
                    )}
                  </div>
                  <h2 className="mt-5 font-bold">{item.recipientName}</h2>
                  <p className="mt-2 text-sm leading-7 text-slate-500">
                    {[item.street, item.buildingInfo, item.neighborhood, item.city]
                      .filter(Boolean)
                      .join(', ')}
                    <br />
                    {item.contactNumber}
                  </p>
                  <div className="mt-5 flex flex-wrap gap-2 border-t border-line pt-5">
                    <Button asChild variant="outline" size="sm">
                      <Link
                        to="/account/addresses/$addressId/edit"
                        params={{ addressId: String(item.savedAddressId) }}
                      >
                        <PenLine size={14} /> Edit
                      </Link>
                    </Button>
                    {!item.isDefault && (
                      <Button
                        size="sm"
                        variant="outline"
                        disabled={defaultState.isLoading}
                        onClick={() => void makeDefault({ addressId: item.savedAddressId ?? 0 })}
                      >
                        <Star size={14} /> Set default
                      </Button>
                    )}
                    <ConfirmAction
                      destructive
                      title="Delete address?"
                      description="This saved address will be permanently removed."
                      trigger={
                        <Button variant="ghost" size="sm">
                          <Trash2 size={14} /> Delete
                        </Button>
                      }
                      onConfirm={() => void remove({ addressId: item.savedAddressId ?? 0 })}
                    />
                  </div>
                </Card>
              ))}
            </div>
          )}
        </section>
      </div>
    </div>
  );
}
