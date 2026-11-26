export function fulfillmentStatusLabel(status: number | null | undefined): string {
  switch (status) {
    case 1: return 'Pending';
    case 2: return 'Processing';
    case 3: return 'Fulfilled';
    case 4: return 'Cancelled';
    default: return 'Unknown';
  }
}
