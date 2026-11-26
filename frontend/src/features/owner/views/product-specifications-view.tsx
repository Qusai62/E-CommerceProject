import { ProductResourceLayout } from '../containers/product-resource-layout';
import { SpecificationManager } from '../components/specification-manager';
export function ProductSpecificationsView({productId}:{productId:number}){
 return <ProductResourceLayout productId={productId} title="Technical specifications">{({product})=><SpecificationManager productId={productId} specs={product.specs}/>}</ProductResourceLayout>;
}
